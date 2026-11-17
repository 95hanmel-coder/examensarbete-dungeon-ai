# Jakten på den Heliga Graalen - Textäventyr av Hanita Melin

Valt alternativ: **Textäventyr / Dungeon Crawler**

## Projektbeskrivning
"Jakten på den Heliga Graalen" är ett textbaserat äventyrsspel inspirerat av legenden om Kung Arthur. Du spelar som riddaren Tristan från Tristan & Isolde, vars uppdrag är att utforska Camelot, samla viktiga föremål och slutligen finna den mytomspunna Heliga Graalen. Målet är att hitta Graalen och sedan återvända till Kapellet.

## Screenshot
![Screenshot](Screenshot%20spel.jpg)
*Skärmbilden visar spelets start i "Runda Bordets Sal" med beskrivning och spelarens olika kommandon för att ta sig till nästa rum & ta föremål etc.*

## Hur man startar programmet

### Förutsättningar
*   .NET 9.0 eller senare
*   En av följande utvecklingsmiljöer: Visual Studio, JetBrains Rider, Visual Studio Code, eller terminal/kommandotolk

### Klona projektet
```bash
git clone https://github.com/Campus-Molndal-CLO25/assignment-oop-95hanmel.git
cd assignment-oop-95hanmel
```

### Starta programmet

**Visual Studio**
1.  Öppna `Inlämning_DungeonCrawler.sln` eller projektmappen.
2.  Tryck `F5` eller klicka på "Start" (grön triangel).

**JetBrains Rider**
1.  Öppna projektmappen genom "File → Open".
2.  Tryck `Ctrl+F5` eller klicka på "Run" (grön triangel).

**Visual Studio Code**
1.  Öppna projektmappen: `code .`
2.  Installera C# Dev Kit-tillägget om det saknas.
3.  Tryck `F5` eller använd "Run → Start Debugging".

**Terminal/Kommandotolk**
```bash
dotnet run
```

## Hur man använder programmet
Programmet är helt textbaserat. Du interagerar genom att skriva kommandon i konsolen.

**Exempel på kommandon:**
*   `gå norr` - För att flytta Tristan norrut.
*   `ta sköld` - För att plocka upp en sköld från rummet.
*   `titta` - För att få en ny beskrivning av det aktuella rummet.
*   `inventory` - För att se vilka föremål Tristan bär på.
*   `hjälp` - För att visa en lista över alla tillgängliga kommandon.
*   `avsluta` - För att avsluta spelet.

Tristan måste hitta den **Heliga Graalen** och ta sig tillbaka till **Kapellet** för att vinna.

## Funktioner

### Grundfunktioner
*   Minst 10 sammankopplade rum
*   Navigation med "gå [riktning]"
*   Föremålshantering: ta föremål, visa inventory, föremål försvinner från rummet, vissa föremål är inte tagbara och är kvar i rummet
*   Unika rumsbeskrivningar
*   Felhantering för ogiltiga kommandon/riktningar
*   Tydligt vinstvillkor (finna Heliga Graalen och återvända till Kapellet)

### Extra funktioner (för VG)
*   **Berikade föremålsinteraktioner:** Specifika dialoger och handlingar för unika föremål, exempelvis Brev med sigill i inventory ger effekt. Excalibur och trolldrycken kan inte tas i inventory *( "utökat beteende")*. Samt utökat rummen från 5-10 med alternativa rutter. 

## Projektstruktur
projektmapp/  
├── Program.cs # Huvudprogrammet som startar spelet  
├── Game.cs # Hanterar spelloopen, kommandon, spelare och värld  
├── Player.cs # Representerar spelaren (position, inventory)  
├── Room.cs # Representerar en plats i världen (beskrivning, föremål, utgångar)  
├── Item.cs # Representerar ett föremål  
├── README.md # Denna fil  
└── reflection.md # Reflektion över projektet

## Teknisk information
*   **Programmeringsspråk:** C#
*   **Framework:** .NET 9.0
*   **Utvecklingsmiljö:** [Visual Studio Code / Visual Studio] 

## Länkar
*   **GitHub Repository:** `https://github.com/orgs/Campus-Molndal-CLO25/repositories/assignment-oop-95hanmel
*   **Din privata fork:** *(Länk)*

