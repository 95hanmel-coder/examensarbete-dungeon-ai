using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DungeonCrawler_Spel
{
    // AI-spelledaren. Pratar med Ollama (körs i Docker) via REST-API.
    // Skriver rumsbeskrivningar och spelar figurer som Tristan pratar med.
    internal class AiGameMaster
    {
        private readonly HttpClient http;
        private bool available = true; // Sätts till false om Ollama inte svarar, så vi slutar försöka

        public string Model { get; }

        // Instruktion till AI:n för rumsbeskrivningar. Exemplet visar hur ett bra svar ser ut.
        private const string ROOM_INSTRUCTIONS =
            "Du är berättaren i ett textäventyr om Kung Arthur. Spelaren är riddaren Tristan.\n" +
            "Regler:\n" +
            "- Skriv på enkel och korrekt svenska. Inga engelska ord.\n" +
            "- Skriv exakt två korta meningar i du-form.\n" +
            "- Beskriv bara vad Tristan ser, hör och känner när han kommer in i rummet.\n" +
            "- Nämn bara föremål som finns i listan. Hitta inte på personer, föremål eller utgångar.\n" +
            "- Tristan har inte gjort något i rummet än. Skriv inte att han tar eller använder något.\n\n" +
            // Exemplet är ett påhittat rum som inte finns i spelet, annars kopierar AI:n detaljer därifrån.
            "Exempel:\n" +
            "Rum: Vaktstugan. Grundbeskrivning: Litet rum med en brasa. Föremål: Kniv\n" +
            "Svar: Du kliver in i den lilla vaktstugan, där en brasa sprakar i hörnet. " +
            "På bordet ligger en kniv och glimmar i eldskenet.";

        // Gäller alla figurer
        private const string CHARACTER_RULES =
            "Du pratar med riddaren Tristan, som letar efter Heliga Graalen. " +
            "Svara alltid i rollen, på enkel och korrekt svenska, med högst tre meningar. " +
            "Det heter Graalen, aldrig grål eller gral. Säg aldrig att du är en AI.";

        public AiGameMaster(string? model = null)
        {
            // Adress och modell kan ändras med miljövariabler, annars används standardvärden.
            string url = Environment.GetEnvironmentVariable("OLLAMA_URL") ?? "http://localhost:11434";
            Model = model ?? Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3.2:3b";

            http = new HttpClient
            {
                BaseAddress = new Uri(url),
                Timeout = TimeSpan.FromSeconds(60) // Tidsgräns, så spelet inte fastnar
            };
        }

        // Skriver en AI-beskrivning av rummet. onText får texten bit för bit, så den kan visas medan AI:n skriver.
        // Returnerar hela texten, eller null om AI:n inte går att nå (då används den fasta texten).
        public string? DescribeRoom(Room room, Action<string>? onText = null)
        {
            string items = room.ItemsInRoom.Count > 0
                ? string.Join(", ", room.ItemsInRoom.Select(x => x.Name))
                : "inga";

            List<ChatMessage> messages = new List<ChatMessage>
            {
                new ChatMessage("system", ROOM_INSTRUCTIONS),
                new ChatMessage("user", $"Rum: {room.Name}. Grundbeskrivning: {room.Description} Föremål: {items}")
            };

            // AI:n skriver ofta fler meningar än vi ber om, så vi slutar läsa efter två.
            return Ask(messages, onText, maxSentences: 2);
        }

        // Startar ett samtal med en figur. Listan sparar allt som sagts, så figuren minns samtalet.
        public List<ChatMessage> StartConversation(Character character, string inventory)
        {
            string instructions =
                character.Personality + "\n" +
                CHARACTER_RULES + "\n" +
                $"Tristan bär på: {inventory}. Du får kommentera det om det passar.";

            return new List<ChatMessage> { new ChatMessage("system", instructions) };
        }

        // Skickar det spelaren säger och returnerar figurens svar, eller null om AI:n inte svarar.
        public string? Talk(List<ChatMessage> conversation, string playerText, Action<string>? onText = null)
        {
            conversation.Add(new ChatMessage("user", playerText));
            string? answer = Ask(conversation, onText, maxSentences: 4);

            if (answer != null)
            {
                conversation.Add(new ChatMessage("assistant", answer));
            }
            return answer;
        }

        private string? Ask(List<ChatMessage> messages, Action<string>? onText, int maxSentences)
        {
            if (!available) return null;

            string text = "";
            try
            {
                // stream = true: Ollama skickar svaret bit för bit, en JSON-rad per bit.
                ChatRequest request = new ChatRequest(Model, messages, true, new ChatOptions(0.6));
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
                {
                    Content = JsonContent.Create(request)
                };

                // AI-anropet görs asynkront, men spelloopen är synkron, därför väntar vi in svaret här.
                // ResponseHeadersRead = börja läsa direkt, vänta inte på hela svaret.
                using HttpResponseMessage response = http.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                using StreamReader reader = new StreamReader(response.Content.ReadAsStream());
                int sentences = 0;
                char lastChar = ' ';
                string? line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "") continue;
                    ChatResponse? part = JsonSerializer.Deserialize<ChatResponse>(line);
                    string piece = part?.Message?.Content ?? "";

                    // Klipp efter maxSentences meningar. "..." räknas som ett meningsslut, inte tre.
                    for (int i = 0; i < piece.Length; i++)
                    {
                        bool isEnd = piece[i] == '.' || piece[i] == '!' || piece[i] == '?';
                        bool prevIsEnd = lastChar == '.' || lastChar == '!' || lastChar == '?';
                        lastChar = piece[i];

                        if (isEnd && !prevIsEnd)
                        {
                            sentences++;
                            if (sentences == maxSentences)
                            {
                                piece = piece.Substring(0, i + 1);
                                break;
                            }
                        }
                    }

                    // Ta bort mellanslag och radbrytningar i början av svaret
                    if (text == "") piece = piece.TrimStart();

                    text += piece;
                    if (piece != "") onText?.Invoke(piece);

                    if (sentences >= maxSentences || part?.Done == true) break; // Att sluta läsa avbryter AI:n
                }
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is IOException)
            {
                // Ollama är inte igång eller svarar för långsamt. Spelet fortsätter med de fasta texterna.
                // Har en del av svaret redan kommit används den.
                if (text == "")
                {
                    available = false;
                    Console.WriteLine("(Spelledaren sover. Fortsätter utan AI.)");
                    return null;
                }
            }

            text = text.Trim();
            return text == "" ? null : text;
        }

        // Format som Ollamas /api/chat förväntar sig
        internal record ChatMessage(
            [property: JsonPropertyName("role")] string Role,
            [property: JsonPropertyName("content")] string Content);

        private record ChatOptions(
            [property: JsonPropertyName("temperature")] double Temperature); // Lägre = mindre påhittig

        private record ChatRequest(
            [property: JsonPropertyName("model")] string Model,
            [property: JsonPropertyName("messages")] List<ChatMessage> Messages,
            [property: JsonPropertyName("stream")] bool Stream,
            [property: JsonPropertyName("options")] ChatOptions Options);

        private record ChatResponse(
            [property: JsonPropertyName("message")] ChatMessage? Message,
            [property: JsonPropertyName("done")] bool Done);
    }
}
