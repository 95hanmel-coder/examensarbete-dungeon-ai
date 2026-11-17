using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace DungeonCrawler_Spel
{
    // AI-spelledaren. Pratar med Ollama (körs i Docker) via REST-API och ber om rumsbeskrivningar.
    internal class AiGameMaster
    {
        private readonly HttpClient http;
        private readonly string model;
        private bool available = true; // Sätts till false om Ollama inte svarar, så vi slutar försöka

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
            if (!available) return null;

            string items = room.ItemsInRoom.Count > 0
                ? string.Join(", ", room.ItemsInRoom.Select(x => x.Name))
                : "inga";

            string prompt =
                "Du är spelledare i ett textäventyr om Kung Arthur. Spelaren är riddaren Tristan. " +
                "Svara bara på svenska, med två korta meningar, i du-form. " +
                "Hitta inte på nya föremål, personer eller utgångar.\n" +
                $"Rum: {room.Name}\n" +
                $"Grundbeskrivning: {room.Description}\n" +
                $"Föremål i rummet: {items}";

            try
            {
                OllamaRequest request = new OllamaRequest(model, prompt, false);
                // AI-anropet görs asynkront, men spelloopen är synkron, därför väntar vi in svaret här.
                HttpResponseMessage response = http.PostAsJsonAsync("/api/generate", request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                OllamaResponse? result = response.Content.ReadFromJsonAsync<OllamaResponse>().GetAwaiter().GetResult();
                string? text = result?.Response?.Trim();
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

        private record OllamaRequest(
            [property: JsonPropertyName("model")] string Model,
            [property: JsonPropertyName("prompt")] string Prompt,
            [property: JsonPropertyName("stream")] bool Stream);

        private record OllamaResponse(
            [property: JsonPropertyName("response")] string? Response);
    }
}
