using System.Text;

namespace DungeonCrawler_Spel
{
    internal class Room
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public List<Item> ItemsInRoom { get; set; }
        public List<Character> Characters { get; set; }

        // Mappar riktningar, vilka rum som är sammankopplade
        public Dictionary<string, Room> Exits { get; set; }

        public Room(string name, string description)
        {
            // Viktig initering för att undvika NullReferenceException när det läggs till/tas bort föremål/exists. 
            Name = name;
            Description = description;
            ItemsInRoom = new List<Item>();
            Characters = new List<Character>();
            Exits = new Dictionary<string, Room>();
        }

        // Fasta beskrivningen plus figurer, föremål och utgångar.
        public string GetDescription()
        {
            return Description + Environment.NewLine + GetDetails();
        }

        // Figurer, föremål och utgångar. Bestäms alltid av koden, aldrig av AI:n.
        public string GetDetails()
        {
            // AI. Effektivt för att bygga strängar i loopar. Istället för result +=
            StringBuilder sb = new StringBuilder();

            // Figurer man kan prata med
            if (Characters.Count > 0)
            {
                sb.AppendLine("Här finns: " + string.Join(", ", Characters.Select(x => x.Name)) + ". (Skriv 'prata med [namn]')");
            }

            // Om det finns föremål...
            if (ItemsInRoom.Count > 0)
            {
                sb.Append("Du ser: ");

                // Gå igenom varje föremål och lägg till dess namn. For = Bestämt antal.
                for (int i = 0; i < ItemsInRoom.Count; i++)
                {
                    sb.Append(ItemsInRoom[i].Name);
                    if (i < ItemsInRoom.Count - 1)
                    {
                        // Lägg till komma mellan föremålen, utom det sista.
                        sb.Append(", ");
                    }
                }
                sb.AppendLine(".");
            }
            else
            {
                sb.AppendLine("Det finns inga föremål här.");
            }

            // Lägg till information om utgångar (riktningar att gå). Om det finns några utgångar...
            if (Exits.Count > 0)
            {
                sb.Append("Det går att gå ");

                // Gå igenom varje key/riktning i Exits-dictionaryn och lägg till den.
                bool first = true;

                foreach (string direction in Exits.Keys)
                {
                    if (!first)
                    {
                        sb.Append(" eller ");
                    }
                    sb.Append(direction);
                    first = false;
                }
                sb.AppendLine(".");
            }
            else
            {
                sb.AppendLine("Det finns inga utgångar från detta rum."); // Behövs ej nu, men framtidssäkerhet
            }

            // Returnera den färdiga strängen.
            return sb.ToString();
        }

        public void RemoveItem(Item item)
        {
            ItemsInRoom.Remove(item);
        }

        public void AddItem(Item item)
        {
            ItemsInRoom.Add(item);
        }
    }
}



