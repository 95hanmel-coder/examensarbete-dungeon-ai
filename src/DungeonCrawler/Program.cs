namespace DungeonCrawler_Spel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Game game = new Game();

            // dotnet run -- --benchmark [modell ...] mäter AI-modellerna i stället för att starta spelet
            if (args.Length > 0 && args[0] == "--benchmark")
            {
                game.RunBenchmark(args.Skip(1).ToArray());
                return;
            }

            game.Start();
        }
    }
}
