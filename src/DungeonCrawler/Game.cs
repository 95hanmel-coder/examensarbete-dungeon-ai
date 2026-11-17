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
            Console.ReadKey();
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
            dragonCave.Exits.Add("väst", throneRoom); // genväg från drakgrottan till tronsalen

            // Lancelots kammare
            lancelotsChamber.Exits.Add("öst", roundTableHall);
            lancelotsChamber.Exits.Add("väst", throneRoom);
            lancelotsChamber.Exits.Add("norr", royalChamber);

            // Tronsalen
            throneRoom.Exits.Add("öst", lancelotsChamber);

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

            switch (verb)
            {
                case "gå": MovePlayer(noun); break;
                case "titta": Console.WriteLine(player.CurrentRoom.GetDescription()); break;
                case "ta": TakeItemFromRoom(noun); break;
                case "inventory": Console.WriteLine(player.GetInventoryDescription()); break;
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

            if (!player.CurrentRoom.Exits.TryGetValue(direction, out Room nextRoom))
            {
                Console.WriteLine("Du försöker gå genom väggen. Tyvärr, Tristan är inte en spökfigur… än.");
                return;
            }

            player.MoveToRoom(nextRoom);
            Console.WriteLine($"Du går {direction} och kommer till {player.CurrentRoom.Name}.");
            DescribeCurrentRoom();
        }

        // Ber AI-spelledaren beskriva rummet. Svarar den inte visas den fasta texten.
        private void DescribeCurrentRoom()
        {
            Console.WriteLine("(Spelledaren funderar...)");
            string? aiText = gameMaster.DescribeRoom(player.CurrentRoom);
            Console.WriteLine(player.CurrentRoom.GetDescription(aiText));
        }


        private void TakeItemFromRoom(string itemName)
        {
            // AI hjälp. Hitta objektet i rummets lista baserat på namn (case-insensitive) med lambda. Ist för foreach. 
            Item itemToTake = player.CurrentRoom.ItemsInRoom.Find(item => item.Name.ToLower() == itemName.ToLower());

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

        private void ShowHelp()
        {
            Console.WriteLine("Tillgängliga kommandon:");
            Console.WriteLine(" gå [riktning] - rör dig i en riktning (norr, syd, öster, väster)");
            Console.WriteLine(" titta - beskriver rummet igen");
            Console.WriteLine(" ta [föremål] - tar ett föremål från rummet");
            Console.WriteLine(" inventory - visar föremålen du bär på");
            Console.WriteLine(" hjälp - visar denna lista");
            Console.WriteLine(" avsluta - avslutar spelet");
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

