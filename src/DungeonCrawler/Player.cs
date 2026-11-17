using System.Text;

namespace DungeonCrawler_Spel
{
    internal class Player
    {
        public Room CurrentRoom { get; set; } = null!; // Sätts när spelet startar, i Game.Start()

        public List<Item> Inventory { get; set; }

        public Player()
        {
            Inventory = new List<Item>();
        }

        public void MoveToRoom(Room newRoom)
        {
            if (newRoom == null) //Nullsäkrar
            {
                Console.WriteLine("Rummet du försöker gå till finns inte!");
                return;
            }
            CurrentRoom = newRoom;
        }
        public void TakeItem(Item item)
        {
            if (item == null)
            {
                Console.WriteLine("Detta föremål finns inte att ta.");
                return;
            }

            if (CurrentRoom == null)
            {
                Console.WriteLine("Du befinner dig inte i något rum just nu.");
                return;
            }

            CurrentRoom.RemoveItem(item);
            Inventory.Add(item);
        }


        public bool HasItem(string itemName)
        {
            // AI hjälp. Kollar om spelaren bär på ett föremål,jämför med namn (använder LINQ + lambda) 
            return Inventory.Any(x => x.Name.ToLower() == itemName.ToLower());
        }

        public string GetInventoryDescription()
        {
            if (Inventory.Count == 0)
                return "Du har inga föremål i ditt inventory.";

            // Start på listan av föremål
            StringBuilder sb = new StringBuilder();
            sb.Append("Du har: ");

            for (int i = 0; i < Inventory.Count; i++)
            {
                if (Inventory[i] == null) continue; // Nullsäkring, hoppa över null-föremål. 

                sb.Append(Inventory[i].Name);

                // Extra effekt om spelaren bär på brevet
                if (Inventory[i].Name == "Brev med sigill")
                {
                    sb.Append(" (Tristans hjärta bultar snabbare...♥)");
                }

                if (i < Inventory.Count - 1)
                    sb.Append(", ");
            }

            sb.AppendLine(".");

            return sb.ToString();
        }
    }
}



