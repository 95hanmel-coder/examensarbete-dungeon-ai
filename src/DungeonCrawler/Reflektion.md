## Planering

Hur planerade du ditt arbete från start till mål?

Mitt arbete startade med att jag gick igenom uppgiftsbeskrivningen. Jag valde Dungeon Crawler-projektet eftersom jag kunde se hur det passade bra med Clean Code och ren struktur med separata klasser. Jag började med att göra flödesschema av grundläggande klasserna jag skulle behöva: `Player`, `Room`, och `Item`. Samt en karta över rummen med navigation i båda riktningar och föremål utplacerade. 

Min plan var att bygga en "MVP" först. Det första målet var: få en spelare att kunna röra sig mellan två rum. När den grundläggande rörelsemekaniken med `Dictionary` för utgångar fungerade, byggde jag vidare. Nästa steg var att lägga till föremål i rummen och implementera `ta`-kommandot. Därefter lade jag till `inventory`-kommandot och slutligen vinstvillkoret. Genom att arbeta stegvis kunde jag testa varje ny funktion för sig och se till att den fungerade innan jag gick vidare. Samt steg för steg kunna förklara och se om det finns utrymme för förbättringar. 

*Spelkarta*
![Tristans karta](Tristans%20karta.png)

## Problem

Vad var den svåraste delen av uppgiften och varför? Hur löste du de problem du stötte på?

- Först konceptuellt var att förstå hur jag skulle koppla ihop rummen. Jag förstod att jag behövde en lista med utgångar, men hur skulle jag koppla en sträng som 'norr' till ett faktiskt `Room`-objekt? Lösningen blev att använda en `Dictionary<string, Room>`, där strängen är riktningen och `Room` är en referens till nästa rum. Det tog ett tag innan det 'klickade'. 

- Ett av de svåraste men mest lärorika momenten var logiken för att plocka upp föremål. Med hjälp av AI använde jag ett **lambda-uttryck** för att göra koden mer kompakt än en manuell loop. Jag löste även en kritisk **dupliceringsbugg** i `Items` genom en dedikerad metod där föremål tas **först** bort från rummets lista och **sedan** läggs till i spelarens inventory. Detta lärde mig vikten av konsekvent hantering för att undvika logiska fel.

- Jag hade även problem med att formulera en snygg beskrivning av rummets innehåll. Min kod blev rörig, jag frågade ChatGPT om ett effektivt sätt att bygga strängar, och fick tipset om att använda `StringBuilder`. Det gjorde min `GetDescription`-metod både renare och mer effektiv. Så med **StringBuilder** tränade jag på att skriva kod som är **optimerad för större datamängder**, även om skillnaden inte märks i mitt lilla spel. Potentiellt framtidssäkra om man utökar spelet.

## Stolthet

Vad är du mest stolt över med ditt projekt?

- Jag är mest stolt över att jag lyckades implementera en tydlig objektorienterad struktur från grunden. Att se hur de olika klasserna –`Game`, `Player`, `Room` och `Item` – samspelade för att skapa en fungerande helhet var en riktig aha-upplevelse. Innan det här projektet hade jag svårt att se den praktiska nyttan med klasser, men nu förstår jag verkligen hur de hjälper till att organisera och strukturera ett större program. Speciellt stolt är jag över hur `Game`-klassen fungerar som en motor som binder ihop allt, medan de andra klasserna bara fokuserar på sin specifika uppgift. (Men mest över specialföremålens lösningar). Det känns som framsteg även om de är små.

## Reflektion för (VG)

### ## Datastrukturer

**Vilka datastrukturer (t.ex. listor, arrays, dictionaries) använde du i ditt program? Varför valde du just dessa? Finns det andra datastrukturer som hade kunnat fungera och vilka för- eller nackdelar hade de haft i just ditt projekt?**

I mitt projekt använde jag primärt tre olika datastrukturer, var och en vald för ett specifikt syfte:

1. **`List<Item>`:** För att hantera föremål i ett rum (`ItemsInRoom`) och i spelarens ägo (`Inventory`) valde jag en generisk lista. Anledningen var att antalet föremål på dessa platser är dynamiskt; spelaren plockar upp och (i en framtida version) kanske släpper föremål. `List<T>` är idealisk här eftersom:

	- Den har en dynamisk storlek som kan växa och krympa efter behov.

	- Den erbjuder enkla och läsbara metoder som `.Add()`, `.Remove()` och `.Find()`, vilket gjorde koden för att manipulera föremål väldigt ren.

	- **Alternativ:** En vanlig **`Array` (`Item[]`)** hade kunnat användas. Nackdelen är att en array har en fast storlek. Jag hade då behövt bestämma ett maximalt antal föremål per rum eller tvingats skapa en ny, större array och kopiera över alla element varje gång ett föremål lades till, vilket är ineffektivt och komplext.

2. **`Dictionary<string, Room>`:** För att hantera ett rums utgångar (`Exits`) valde jag en Dictionary. Detta var ett medvetet val för att effektivt koppla en textsträng (en riktning som `"norr"`) till en specifik `Room`-instans. Fördelarna var:

	-  **Omedelbar uppslagning:** Att hitta rummet i en viss riktning är snabbt (`Exits["norr"]`). Detta är mycket effektivare än att loopa igenom en lista för att hitta rätt utgång.

	- **Flexibilitet:** Det är enkelt att lägga till nya utgångar utan att ändra klassens struktur, även med ovanliga riktningar som "upp" eller "sydväst".

	- **Alternativ:** Jag hade kunnat använda en **`List<Exit>`**, där `Exit` var en egen klass med egenskaperna `Direction` och `TargetRoom`. Nackdelen är att för att hitta rummet åt norr hade jag behövt loopa igenom listan och jämföra varje `exit.Direction` med "norr". Detta är långsammare (O(n) istället för O(1)) och ger mer omständlig kod.

3. **`string[]` (String Array):** Denna datastruktur användes implicit när jag hanterade användarens kommandon med `command.Split(' ', 2)`. Detta delar upp inmatningen i en array av strängar (t.ex. `["gå", "norr"]`). En array var perfekt här eftersom kommandot alltid består av ett fast, litet antal delar (verb och ibland ett substantiv), så den dynamiska storleken hos en lista var onödig.


---

### ## Clean Code och Struktur

**Hur har du arbetat för att göra din kod "ren" och välstrukturerad? Reflektera över hur du har organiserat dina klasser och metoder för att göra koden läsbar, återanvändbar och lätt att underhålla (modularitet). Ge konkreta exempel från din kod.**

Jag har strävat efter att följa principer för Clean Code genom hela projektet, främst genom en tydlig objektorienterad struktur och någorlunda genomtänkt namngivning.

- **Tydligt ansvar (Single Responsibility Principle):** Min största ansträngning låg i att ge varje klass ett enda, tydligt ansvar.
    
    - **Exempel:** `Player`-klassen ansvarar _enbart_ för spelarens tillstånd: dess nuvarande rum (`CurrentRoom`) och dess föremål (`Inventory`). Den vet ingenting om hur världen är uppbyggd eller hur kommandon tolkas. `Room`-klassen ansvarar bara för ett rums tillstånd (beskrivning, föremål, utgångar). `Game`-klassen agerar som dirigenten som binder ihop allt, hanterar spellogik och användarinput. Denna separation gör att om jag vill ändra hur ett rum beskrivs, behöver jag bara ändra i `Room`-klassen utan att riskera att förstöra spelarlogiken.
        
- **Tydlig namngivning:** Jag har konsekvent använt namn som beskriver syftet.
    
    - **Exempel på klasser:** `Player`, `Room`, `Item` är substantiv som beskriver vad de representerar.
        
    - **Exempel på metoder:** `CreateWorld()`, `ProcessCommand()`, `GetInventoryDescription()` är verbfraser som beskriver vad de _gör_.
        
    - **Exempel på variabler:** Istället för generiska namn som `temp` eller `obj` har jag använt `itemToTake` och `nextRoom` för att omedelbart göra kodens intention tydlig.
        
- **Undvikande av upprepning (DRY - Don't Repeat Yourself):**
    
    - **Exempel:** Beskrivningen av ett rum behövs på flera ställen (när spelet startar, när spelaren går till ett nytt rum, och med `titta`-kommandot). Istället för att upprepa denna logik, skapade jag en enda metod, `Room.GetDescription()`, som dynamiskt bygger beskrivningen. Om jag i framtiden vill ändra hur rum beskrivs, behöver jag bara ändra på detta enda ställe.
        
- **Kommentering:** Jag har använt kommentarer "sparsamt", primärt för att förklara _varför_ jag har gjort ett visst val (som att förklara ordningen i `TakeItem`-metoden) eller för att dokumentera var jag fått extern hjälp, vilket är en del av god akademisk praxis.
    

---

### ## Framtida utveckling

**Kan du fortsätta att utveckla ditt projekt utan att behöva göra om allt från grunden? Hur har du designat din kod för att den ska vara möjlig att bygga vidare på i framtiden?**

Ja, jag anser att projektets design är robust och möjliggör betydande framtida utveckling utan att grundarkitekturen behöver skrivas om. Detta beror främst på den tydliga separationen av ansvar mellan klasserna.

- **Vad som är LÄTT att utöka:**
    
    - **Lägga till mer innehåll (Rum och Föremål):** Detta är den enklaste tänkbara utökningen. Eftersom hela världens skapande är isolerat i `Game.CreateWorld()`-metoden, kan jag lägga till dussintals nya rum, föremål och kopplingar där utan att röra en enda rad kod i `Player`- eller `Room`-klasserna. Logiken är helt frikopplad från datan, vilket är en mycket skalbar design. Man skulle till och med kunna bygga ut `CreateWorld` till att läsa in all data från en textfil eller JSON-fil.
        
    - **Lägga till nya kommandon:** `switch`-satsen i `ProcessCommand`-metoden är designad för att vara utbyggbar. Att lägga till ett nytt kommando, som `använd [föremål]` eller `prata med [person]`, skulle bara innebära att lägga till ett nytt `case` och anropa en ny metod. Spellopen och den grundläggande strukturen påverkas inte.
        
- **Vad som är SVÅRT att ändra:**
    
    - **Införa multiplayer:** Den nuvarande arkitekturen är fundamentalt byggd kring en enda instans av `Player`. Att införa multiplayer skulle kräva en omfattande omstrukturering. `Game`-klassen skulle behöva hantera en lista av spelare, en turordningslogik, och hur spelare interagerar med varandra i samma rum. Detta var inte ett designmål från början, och koden reflekterar det.
        
    - **Införa dynamiska fiender (NPCs):** Att lägga till fiender som kan röra sig mellan rum och agera på egen hand skulle vara en stor utmaning. Det skulle kräva en ny `Monster`-klass, någon form av AI-logik, och en ombyggnation av spellopen i `Game` för att hantera både spelarens tur och monstrens tur. Världen är just nu helt statisk, och att göra den dynamisk är en stor förändring.
        

Sammanfattningsvis har separationen av ansvar gjort koden mycket modulär. Att bygga ut spelets **innehåll och funktioner** är enkelt, medan att ändra dess **grundläggande antaganden** (som single-player och en statisk värld) skulle vara svårt, vilket är en förväntad och acceptabel kompromiss i en sådan här design.

---
