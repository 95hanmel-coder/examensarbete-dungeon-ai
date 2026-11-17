using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace DungeonCrawler_Spel
{
    // AI-spelledaren. Pratar med Ollama (körs i Docker) via REST-API.
    // Skriver rumsbeskrivningar och spelar figurer som Tristan pratar med.
    internal class AiGameMaster
    {
        private readonly HttpClient http;
        private readonly string model;
        private bool available = true; // Sätts till false om Ollama inte svarar, så vi slutar försöka

        // Instruktion till AI:n för rumsbeskrivningar. Exemplet visar hur ett bra svar ser ut.
        private const string ROOM_INSTRUCTIONS =
            "Du är berättaren i ett textäventyr om Kung Arthur. Spelaren är riddaren Tristan.\n" +
            "Regler:\n" +
            "- Skriv på enkel och korrekt svenska. Inga engelska ord.\n" +
            "- Skriv exakt två korta meningar i du-form.\n" +
            "- Beskriv bara vad Tristan ser, hör och känner när han kommer in i rummet.\n" +
            "- Nämn bara föremål som finns i listan. Hitta inte på personer, föremål eller utgångar.\n" +
            "- Tristan har inte gjort något i rummet än. Skriv inte att han tar eller använder något.\n\n" +
            "Exempel:\n" +
            "Rum: Kapellet. Grundbeskrivning: Heligt rum med altare och statyer. Föremål: Fackla\n" +
            "Svar: Du kliver in i det tysta kapellet, där stenstatyer vakar över ett gammalt altare. " +
            "En fackla fladdrar i väggen och kastar långa skuggor över golvet.";

        public AiGameMaster()
        {
            // Adress och modell kan ändras med miljövariabler, annars används standardvärden.
            string url = Environment.GetEnvironmentVariable("OLLAMA_URL") ?? "http://localhost:11434";
            model = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3.2:3b";

            http = new HttpClient
            {
                BaseAddress = new Uri(url),
                Timeout = TimeSpan.FromSeconds(30) // Tidsgräns, så spelet inte fastnar
            };
        }

        // Returnerar en AI-skriven beskrivning, eller null om AI:n inte går att nå (då används den fasta texten).
        public string? DescribeRoom(Room room)
        {
            string items = room.ItemsInRoom.Count > 0
                ? string.Join(", ", room.ItemsInRoom.Select(x => x.Name))
                : "inga";

            List<ChatMessage> messages = new List<ChatMessage>
            {
                new ChatMessage("system", ROOM_INSTRUCTIONS),
                new ChatMessage("user", $"Rum: {room.Name}. Grundbeskrivning: {room.Description} Föremål: {items}")
            };

            return Ask(messages);
        }

        // Startar ett samtal med en figur. Listan sparar allt som sagts, så figuren minns samtalet.
        public List<ChatMessage> StartConversation(Character character)
        {
            string instructions =
                character.Personality + "\n" +
                "Du pratar med riddaren Tristan, som letar efter Heliga Graalen. " +
                "Svara alltid i rollen, på enkel och korrekt svenska, med högst tre meningar. " +
                "Säg aldrig att du är en AI.";

            return new List<ChatMessage> { new ChatMessage("system", instructions) };
        }

        // Skickar det spelaren säger och returnerar figurens svar, eller null om AI:n inte svarar.
        public string? Talk(List<ChatMessage> conversation, string playerText)
        {
            conversation.Add(new ChatMessage("user", playerText));
            string? answer = Ask(conversation);

            if (answer != null)
            {
                conversation.Add(new ChatMessage("assistant", answer));
            }
            return answer;
        }

        private string? Ask(List<ChatMessage> messages)
        {
            if (!available) return null;

            try
            {
                ChatRequest request = new ChatRequest(model, messages, false, new ChatOptions(0.6));
                // AI-anropet görs asynkront, men spelloopen är synkron, därför väntar vi in svaret här.
                HttpResponseMessage response = http.PostAsJsonAsync("/api/chat", request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                ChatResponse? result = response.Content.ReadFromJsonAsync<ChatResponse>().GetAwaiter().GetResult();
                string? text = result?.Message?.Content?.Trim();
                return string.IsNullOrWhiteSpace(text) ? null : text;
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                // Ollama är inte igång eller svarar för långsamt. Spelet fortsätter med de fasta texterna.
                available = false;
                Console.WriteLine("(Spelledaren sover. Fortsätter utan AI.)");
                return null;
            }
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
            [property: JsonPropertyName("message")] ChatMessage? Message);
    }
}
