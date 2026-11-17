using System.Diagnostics;

namespace DungeonCrawler_Spel
{
    internal class Game
    {
        private Player player;
        private bool gameOver = false;
        private Dictionary<string, Room> allRooms;
        private AiGameMaster gameMaster = new AiGameMaster();
        private const string WINNING_ITEM_NAME = "Heliga Graalen";
        private const string WINNING_ROOM_NAME = "Kapellet";

        public Game()
        {
            player = new Player();
            allRooms = new Dictionary<string, Room>();
            CreateWorld();
        }

        public void Start()
        {
            player.CurrentRoom = allRooms["Runda Bordets Sal"];
            Console.WriteLine("Välkommen till Jakten på Heliga Graalen! Du spelar som Tristan.");
            Console.WriteLine("Skriv 'hjälp' för att se tillgängliga kommandon.\n");


            Console.WriteLine($"Du befinner dig i {player.CurrentRoom.Name}.");
            DescribeCurrentRoom();

            while (!gameOver)
            {
                Console.Write("> ");
                string input = Console.ReadLine()?.ToLower() ?? ""; // Läser in kommandon, null-säker
                Console.WriteLine();
                ProcessCommand(input);
                CheckWinCondition();
            }

            Console.WriteLine("Tack för att du spelade!");
            // Vänta på tangent bara när någon spelar på riktigt (inte i tester eller Docker utan tangentbord)
            if (!Console.IsInputRedirected)
            {
                Console.ReadKey();
            }
        }

        private void CreateWorld()
        {

            Room roundTableHall = new Room("Runda Bordets Sal", "Riddarna sitter vid det stora runda bordet. Diskussioner om Graalen pågår.");
            Room lancelotsChamber = new Room("Lancelots Kammare", "Elegant rum med säng och vapensköldar.");
            Room throneRoom = new Room("Tronsalen", "Storslagen sal med tron och pampiga dekorationer.");
            Room royalChamber = new Room("Kungliga Kammaren", "Dyrbar inredning och Merlins spådomsbok ligger öppnad på bordet.");
            Room chapel = new Room("Kapellet", "Heligt rum med altare och statyer. En urholkning på altaret syns.");
            Room dragonCave = new Room("Drakgrottan", "Mörk grotta. Du känner att något eller någon iakttar dig.");
            Room camelotsCourtyard = new Room("Camelots Borggård", "En öppen gård med fontän och blommor.");
            Room avalonGarden = new Room("Avalons Trädgård", "Lummig trädgård med exotiska växter.");
            Room merlinsTower = new Room("Merlins Torn", "Mystiskt torn med böcker och trolldrycker. Ett magiskt föremål bubblar i en flaska.");
            Room nimueChamber = new Room("Nimues Spegelsal", "Ett rum fyllt med speglar och mystik. Doften av syrén och kråsbär fyller rummet."); // Easter egg


            allRooms.Add(roundTableHall.Name, roundTableHall);
            allRooms.Add(lancelotsChamber.Name, lancelotsChamber);
            allRooms.Add(throneRoom.Name, throneRoom);
            allRooms.Add(royalChamber.Name, royalChamber);
            allRooms.Add(chapel.Name, chapel);
            allRooms.Add(dragonCave.Name, dragonCave);
            allRooms.Add(camelotsCourtyard.Name, camelotsCourtyard);
            allRooms.Add(avalonGarden.Name, avalonGarden);
            allRooms.Add(merlinsTower.Name, merlinsTower);
            allRooms.Add(nimueChamber.Name, nimueChamber);


            // Runda bordet
            roundTableHall.Exits.Add("syd", chapel);
            roundTableHall.Exits.Add("väst", lancelotsChamber);
            roundTableHall.Exits.Add("öst", camelotsCourtyard);

            // Kapellet
            chapel.Exits.Add("norr", roundTableHall);
            chapel.Exits.Add("syd", dragonCave);

            // Drakgrottan
            dragonCave.Exits.Add("norr", chapel);
            dragonCave.Exits.Add("väst", throneRoom); // genväg mellan drakgrottan och tronsalen, åt båda hållen

            // Lancelots kammare
            lancelotsChamber.Exits.Add("öst", roundTableHall);
            lancelotsChamber.Exits.Add("väst", throneRoom);
            lancelotsChamber.Exits.Add("norr", royalChamber);

            // Tronsalen
            throneRoom.Exits.Add("öst", lancelotsChamber);
            throneRoom.Exits.Add("syd", dragonCave); // tillbaka genom genvägen

            // Kungliga kammaren
            royalChamber.Exits.Add("syd", lancelotsChamber);

            // Camelots borggård
            camelotsCourtyard.Exits.Add("väst", roundTableHall);
            camelotsCourtyard.Exits.Add("norr", avalonGarden);
            camelotsCourtyard.Exits.Add("öst", merlinsTower);

            // Avalon trädgård
            avalonGarden.Exits.Add("syd", camelotsCourtyard);

            // Merlins torn
            merlinsTower.Exits.Add("väst", camelotsCourtyard);
            merlinsTower.Exits.Add("norr", nimueChamber);

            // Nimue spegelkammare
            nimueChamber.Exits.Add("syd", merlinsTower);


            // Figurer som AI-spelledaren spelar
            merlinsTower.Characters.Add(new Character("Merlin",
                "Du är trollkarlen Merlin, gammal, vis och lite lurig. Du talar gåtfullt. " +
                "Du vet att Heliga Graalen finns i Avalons Trädgård, norr om Camelots Borggård, " +
                "och att den ska bäras till Kapellet. Säg det aldrig rakt ut, ge bara ledtrådar."));
            nimueChamber.Characters.Add(new Character("Nimue",
                "Du är Nimue, Sjöns Dam, som en gång gav kung Arthur svärdet Excalibur. " +
                "Du är vänlig men hemlighetsfull och talar om speglar, vatten och öden. " +
                "Du vet inte var Graalen finns, men du tycker att Tristan ska fråga Merlin."));
            dragonCave.Characters.Add(new Character("Draken",
                "Du är en uråldrig, trött drake som bor i grottan under Camelot. Du pratar långsamt och lite grinigt " +
                "och vill mest sova. Du har tappat ett fjäll som du är fåfäng över. " +
                "Du vet att Graalen inte finns i din grotta, men du vet inte var den är."));


            Item excalibur = new Item("Excalibur", "");
            Item potion = new Item("Trolldryck", "");
            Item merlinsStaff = new Item("Trollstav", "En knotig stav med inristade runor, fylld av gammal magi 🔮");
            Item shield = new Item("Sköld", "Nu kan draken inte bita dig… åtminstone inte på riktigt 🛡");
            Item letter = new Item("Brev med sigill", "Guineveres sigill. Tristan läser det inte, då vissa hemligheter kanske är bättre att lämna ifred.");
            Item torch = new Item("Fackla", "Lyser upp mörka hörn 🔥");
            Item graal = new Item(WINNING_ITEM_NAME, "Den Heliga Graalen 🏆! Tristan har funnit den");
            Item dragonScale = new Item("Drakfjäll", "Ett glänsande fjäll från draken 🐉 – hårt som stål men skiner som solen.");
            Item perfume = new Item("Parfymflaska", "Elegant svartlila flaska med doft av syrén och krusbär 🌸 kanske inspirerad av en annan legend.");
            Item mordredLetter = new Item("Öppet brev", "Ihopvikt, vaxet pryds av ett svärd korsat av en ros 🌹 – Mordreds märke.");
            Item percevalHelmet = new Item("Riddarhjälm", "Percevals hjälm bär ett lejonemblem 🦁 - symboliserar mod och ära");
            Item galahadMedaljong = new Item("Medaljong", "Galahads medaljong 🌟 som strålar av ädelhet, kanske tappad av en riddare på väg mot sitt öde.");

            roundTableHall.AddItem(excalibur);
            merlinsTower.AddItem(potion);
            merlinsTower.AddItem(merlinsStaff);
            lancelotsChamber.AddItem(shield);
            lancelotsChamber.AddItem(letter);
            chapel.AddItem(torch);
            avalonGarden.AddItem(graal);
            dragonCave.AddItem(dragonScale);
            nimueChamber.AddItem(perfume);
            royalChamber.AddItem(mordredLetter);
            camelotsCourtyard.AddItem(percevalHelmet);
            throneRoom.AddItem(galahadMedaljong);
        }

        private void ProcessCommand(string command)
        {
            string[] parts = command.Split(' ', 2); // Dela på första mellanslag
            string verb = parts[0];
            string noun = parts.Length > 1 ? parts[1] : ""; // Kan vara föremål eller riktning

            // Bara en riktning, t.ex. "syd", räcker också för att gå
            if (NormalizeDirection(verb) != null)
            {
                MovePlayer(verb);
                return;
            }

            switch (verb)
            {
                case "gå": MovePlayer(noun); break;
                case "titta": DescribeCurrentRoom(); break;
                case "ta": TakeItemFromRoom(noun); break;
                case "inventory": Console.WriteLine(player.GetInventoryDescription()); break;
                case "prata": TalkTo(noun); break;
                case "hjälp": ShowHelp(); break;
                case "avsluta": gameOver = true; Console.WriteLine("Avslutar spelet..."); break;
                default: Console.WriteLine("Förstår inte kommandot. Skriv 'hjälp' för en lista över kommandon."); break;
            }
        }

        private void MovePlayer(string direction)
        {
            if (player.CurrentRoom == null) // Nullsäkrar
            {
                Console.WriteLine("Du är inte i något rum just nu.");
                return;
            }

            string? normalized = NormalizeDirection(direction);
            if (normalized == null)
            {
                Console.WriteLine("Vart vill du gå? Skriv till exempel 'gå norr' eller bara 'norr'.");
                return;
            }
            direction = normalized;

            if (!player.CurrentRoom.Exits.TryGetValue(direction, out Room? nextRoom))
            {
                Console.WriteLine("Du försöker gå genom väggen. Tyvärr, Tristan är inte en spökfigur… än.");
                return;
            }

            player.MoveToRoom(nextRoom);
            Console.WriteLine($"Du går {direction} och kommer till {player.CurrentRoom.Name}.");
            DescribeCurrentRoom();
        }

        // Gör om olika sätt att skriva en riktning till det namn rummen använder. null = ingen riktning.
        private static string? NormalizeDirection(string word)
        {
            switch (word.Trim())
            {
                case "norr": case "nord": case "n": return "norr";
                case "syd": case "söder": case "s": return "syd";
                case "öst": case "öster": case "ö": return "öst";
                case "väst": case "väster": case "v": return "väst";
                default: return null;
            }
        }

        // Ber AI-spelledaren beskriva rummet. Texten visas medan AI:n skriver. Svarar den inte visas den fasta texten.
        private void DescribeCurrentRoom()
        {
            string? aiText = gameMaster.DescribeRoom(player.CurrentRoom, Console.Write);

            if (aiText == null)
            {
                Console.Write(player.CurrentRoom.Description);
            }
            Console.WriteLine();
            Console.WriteLine(player.CurrentRoom.GetDetails());
        }


        private void TakeItemFromRoom(string itemName)
        {
            // AI hjälp. Hitta objektet i rummets lista baserat på namn (case-insensitive) med lambda. Ist för foreach. 
            Item? itemToTake = player.CurrentRoom.ItemsInRoom.Find(item => item.Name.ToLower() == itemName.ToLower());

            // Räcker att skriva början av namnet, t.ex. "ta brev" för "Brev med sigill"
            if (itemToTake == null && itemName != "")
            {
                itemToTake = player.CurrentRoom.ItemsInRoom.Find(item => item.Name.ToLower().StartsWith(itemName.ToLower()));
            }

            // Specialobjekt som inte kan tas
            if (itemToTake != null)
            {
                if (itemToTake.Name == "Excalibur")
                {
                    Console.WriteLine("Excalibur glänser i ljuset 🗡️ Kanske är det du, Tristan, som är värdig att dra det ur stenen?");
                    return;
                }

                if (itemToTake.Name == "Trolldryck")
                {
                    Console.WriteLine("Trolldrycken bubblar mystiskt, men Tristan låter den stå kvar.");
                    return;
                }

                player.TakeItem(itemToTake);
                Console.WriteLine($"Du tar {itemToTake.Name}. {itemToTake.Description}");
            }

            else
            {
                Console.WriteLine($"Objektet finns inte här. Kanske Merlin har gömt det!");
            }
        }

        private void TalkTo(string text)
        {
            // "prata med merlin" -> noun blir "med merlin", ta bort "med"
            string name = text.StartsWith("med ") ? text.Substring(4) : text;
            Character? character = player.CurrentRoom.Characters.Find(x => x.Name.ToLower() == name.Trim());

            if (character == null)
            {
                Console.WriteLine("Här finns ingen med det namnet. Tristan pratar med väggen, men väggen svarar inte.");
                return;
            }

            string inventory = player.Inventory.Count > 0 ? string.Join(", ", player.Inventory.Select(x => x.Name)) : "ingenting";
            List<AiGameMaster.ChatMessage> conversation = gameMaster.StartConversation(character, inventory);
            Console.WriteLine($"Du pratar med {character.Name}. Skriv 'hejdå' för att sluta prata.\n");

            while (true)
            {
                Console.Write("Tristan: ");
                string said = Console.ReadLine()?.Trim() ?? "hejdå"; // null = inmatningen tog slut
                if (said.ToLower() == "hejdå" || said == "") break;

                Console.Write($"{character.Name}: ");
                string? answer = gameMaster.Talk(conversation, said, Console.Write); // Svaret visas medan AI:n skriver

                if (answer == null)
                {
                    Console.WriteLine($"({character.Name} ser fundersam ut men säger ingenting.)");
                    break;
                }
                Console.WriteLine("\n");
            }

            Console.WriteLine($"Du lämnar {character.Name} och ser dig omkring i rummet igen.");
        }

        private void ShowHelp()
        {
            Console.WriteLine("Tillgängliga kommandon:");
            Console.WriteLine(" gå [riktning] - rör dig i en riktning (norr, syd, öst, väst). Det räcker att skriva bara riktningen, t.ex. 'syd'");
            Console.WriteLine(" titta - beskriver rummet igen");
            Console.WriteLine(" ta [föremål] - tar ett föremål från rummet");
            Console.WriteLine(" inventory - visar föremålen du bär på");
            Console.WriteLine(" prata med [namn] - pratar med en figur i rummet");
            Console.WriteLine(" hjälp - visar denna lista");
            Console.WriteLine(" avsluta - avslutar spelet");
        }

        // Mätläge: dotnet run -- --benchmark [modell ...]
        // Ber varje modell beskriva alla rum och svara Merlin en fråga, och mäter tiden. Skriver resultatet som Markdown.
        public void RunBenchmark(string[] models)
        {
            if (models.Length == 0) models = new[] { new AiGameMaster().Model };

            Console.WriteLine($"# Jämförelse av AI-modeller ({DateTime.Now:yyyy-MM-dd})\n");
            Console.WriteLine("Samma instruktioner som i spelet. Tid till första ordet = hur länge spelaren väntar innan texten börjar synas.\n");

            foreach (string model in models)
            {
                AiGameMaster ai = new AiGameMaster(model);
                Console.WriteLine($"## {model}\n");

                // Första anropet laddar in modellen i minnet och tar längre tid. Mäts för sig.
                Measure(() => ai.DescribeRoom(allRooms["Kapellet"]), out double coldStart, out _);
                if (coldStart < 0)
                {
                    Console.WriteLine("Modellen svarade inte. Är den nedladdad?\n");
                    continue;
                }
                Console.WriteLine($"Kallstart (modellen laddas in i minnet): {coldStart:N1} s\n");

                Console.WriteLine("| Rum | Första ordet | Totalt | Beskrivning |");
                Console.WriteLine("|---|---|---|---|");
                List<double> firstWords = new List<double>();
                List<double> totals = new List<double>();

                foreach (Room room in allRooms.Values)
                {
                    string? text = null;
                    double first = -1;
                    Stopwatch watch = Stopwatch.StartNew();
                    text = ai.DescribeRoom(room, piece => { if (first < 0) first = watch.Elapsed.TotalSeconds; });
                    double total = watch.Elapsed.TotalSeconds;

                    firstWords.Add(first);
                    totals.Add(total);
                    Console.WriteLine($"| {room.Name} | {first:N1} s | {total:N1} s | {text?.Replace("\n", " ").Replace("|", "/")} |");
                }

                Console.WriteLine($"\n**Medel:** första ordet {firstWords.Average():N1} s, totalt {totals.Average():N1} s\n");

                // Merlin
                Character merlin = allRooms["Merlins Torn"].Characters[0];
                List<AiGameMaster.ChatMessage> conversation = ai.StartConversation(merlin, "Fackla, Sköld");
                Stopwatch merlinWatch = Stopwatch.StartNew();
                string? answer = ai.Talk(conversation, "Var finns Heliga Graalen?");
                Console.WriteLine($"**Merlin** ({merlinWatch.Elapsed.TotalSeconds:N1} s): \"Var finns Heliga Graalen?\"\n");
                Console.WriteLine($"> {answer?.Replace("\n", " ")}\n");
            }
        }

        // Mäter hur lång tid ett AI-anrop tar. seconds = -1 om AI:n inte svarade.
        private static void Measure(Func<string?> call, out double seconds, out string? result)
        {
            Stopwatch watch = Stopwatch.StartNew();
            result = call();
            seconds = result == null ? -1 : watch.Elapsed.TotalSeconds;
        }

        private void CheckWinCondition()
        {
            if (player.HasItem(WINNING_ITEM_NAME) && player.CurrentRoom.Name == WINNING_ROOM_NAME)
            {
                Console.WriteLine("------------------------------------------------------------------");
                Console.WriteLine($"GRATTIS! Du har hittat {WINNING_ITEM_NAME} och tagit dig till {WINNING_ROOM_NAME}!");
                Console.WriteLine("Du har vunnit äventyret!");
                Console.WriteLine("------------------------------------------------------------------");
                gameOver = true;
            }
        }
    }
}

