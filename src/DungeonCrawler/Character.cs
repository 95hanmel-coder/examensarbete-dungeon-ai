namespace DungeonCrawler_Spel
{
    // En figur i ett rum som Tristan kan prata med. AI-spelledaren spelar figuren.
    internal class Character
    {
        public string Name { get; set; }

        // Beskriver vem figuren är och vad den vet. Skickas till AI:n som instruktion.
        public string Personality { get; set; }

        public Character(string name, string personality)
        {
            Name = name;
            Personality = personality;
        }
    }
}
