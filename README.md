# Jakten på den Heliga Graalen – med AI-spelledare

Ett textäventyr i C# där du spelar riddaren Tristan och letar efter Heliga Graalen i Camelot. Spelet har en **AI-spelledare** som skriver rumsbeskrivningarna och spelar figurer som du kan prata med, till exempel trollkarlen Merlin.

AI:n körs **lokalt på din egen dator** med [Ollama](https://ollama.com) i Docker. Inget kostar pengar, inga konton eller API-nycklar behövs, och ingenting skickas ut på internet.

Examensarbete av Hanita Melin. Projektplanen finns i [docs/project-plan.md](docs/project-plan.md).

---

## För användare

### Det här behöver du

- [Docker Desktop](https://www.docker.com/products/docker-desktop/), som ska vara igång
- Cirka 6 GB ledigt diskutrymme för AI-modellen
- En dator med minst 16 GB RAM (med 8 GB, använd den mindre modellen, se *Inställningar*)

### Starta spelet

1. Ladda ner projektet. Klicka på den gröna knappen **Code → Download ZIP** på GitHub och packa upp filen, eller använd git:
   ```
   git clone https://github.com/95hanmel-coder/examensarbete-dungeon-ai.git
   cd examensarbete-dungeon-ai
   ```
2. Öppna PowerShell eller en terminal i mappen och skriv:
   ```
   docker compose run --build --rm game
   ```
   Första gången tar det några minuter, eftersom AI-modellen (cirka 3,4 GB) laddas ner. Efter det startar spelet på några sekunder.

3. När du har spelat klart kan du stänga av AI-tjänsten:
   ```
   docker compose down
   ```

### Så spelar du

| Kommando | Vad det gör |
|---|---|
| `norr`, `syd`, `öst`, `väst` (eller `gå norr` osv.) | Gå i en riktning |
| `titta` | Beskriv rummet igen |
| `ta [föremål]`, t.ex. `ta sköld` | Ta upp ett föremål. Det räcker med början av namnet |
| `inventory` | Visa vad du bär på |
| `prata med [namn]`, t.ex. `prata med merlin` | Prata med en figur. Skriv `hejdå` för att sluta |
| `hjälp` | Visa alla kommandon |
| `avsluta` | Avsluta spelet |

**Mål:** Hitta Heliga Graalen och bär den till Kapellet.

**Tips:** Fråga Merlin i Merlins Torn om var Graalen finns. Han svarar i gåtor.

### Om något krånglar

| Problem | Lösning |
|---|---|
| `docker: command not found` eller `cannot connect to the Docker daemon` | Starta Docker Desktop och vänta tills den säger att den är igång |
| Spelet skriver "(Spelledaren sover. Fortsätter utan AI.)" | AI:n svarade inte. Spelet fungerar ändå, med enklare texter. Kontrollera att Docker Desktop är igång och starta om spelet |
| Det första svaret tar lång tid | Helt normalt. AI-modellen laddas in i minnet första gången, det tar cirka 15 sekunder |
| Det tar lång tid innan texten börjar synas | Datorn är långsam eller går på batteri. Sätt i laddaren, eller byt till den snabbare modellen: `$env:OLLAMA_MODEL = "llama3.2:3b"` innan du startar spelet |

---

## För utvecklare

### Så hänger delarna ihop

```
┌──────────────┐   HTTP POST /api/chat (JSON)   ┌──────────────┐      ┌──────────────┐
│  Spelet (C#) │ ─────────────────────────────► │    Ollama    │ ───► │  gemma3:4b   │
│  container   │ ◄──────── svar ord för ord ─── │  container   │      │ (AI-modell)  │
└──────────────┘     (streaming, JSON-rader)    └──────────────┘      └──────────────┘
```

- **Spelets regler bestäms av C#-koden:** rum, utgångar, föremål och vinstvillkor. AI:n skriver bara text och kan inte ändra vad som händer i spelet.
- **Streaming:** Ollama skickar svaret bit för bit, och spelet visar texten medan den skrivs, så att väntan känns kortare.
- **Reservläge:** om Ollama inte är igång, eller inte har svarat färdigt inom 60 sekunder, visar spelet de fasta texterna och slutar fråga AI:n.
- AI-svaren för rummen klipps efter två meningar i koden, eftersom modellen ofta skriver längre än den blir ombedd. Spelet slutar då läsa, vilket också avbryter AI:n och sparar tid.
- **Figurerna** (Merlin, Nimue, Draken) minns vad som sagts under samtalet och vet vad Tristan bär på.

### Projektstruktur

```
examensarbete-dungeon-ai/
├── docker-compose.yml        # Startar Ollama, laddar ner modellen och kör spelet
├── docs/
│   ├── project-plan.md       # Projektplan
│   ├── loggbok.md            # Loggbok, vecka för vecka
│   └── model-comparison.md   # Mätning och jämförelse av AI-modeller
└── src/DungeonCrawler/
    ├── Dockerfile            # Bygger spelet till en container
    ├── Program.cs            # Startpunkt
    ├── Game.cs               # Spelloop, kommandon, kartan
    ├── AiGameMaster.cs       # Pratar med Ollama: rumsbeskrivningar och samtal
    ├── Character.cs          # Figur som AI:n spelar (Merlin, Nimue, Draken)
    ├── Room.cs               # Rum med föremål, figurer och utgångar
    ├── Player.cs             # Spelaren och inventory
    └── Item.cs               # Föremål
```

### Köra spelet utan Docker (för utveckling)

Kräver [.NET 10 SDK](https://dotnet.microsoft.com/download).

```
docker compose up -d ollama          # starta bara AI-tjänsten
docker compose run --rm model        # ladda ner modellen (första gången)
cd src/DungeonCrawler
dotnet run
```

### Inställningar

Sätts som miljövariabler, till exempel i PowerShell: `$env:OLLAMA_MODEL = "llama3.2:3b"`

| Variabel | Standardvärde | Vad den gör |
|---|---|---|
| `OLLAMA_URL` | `http://localhost:11434` (i Docker: `http://ollama:11434`) | Adress till Ollama |
| `OLLAMA_MODEL` | `gemma3:4b` | Vilken AI-modell som används |

Byta till den snabbare modellen för hela lösningen: `$env:OLLAMA_MODEL = "llama3.2:3b"; docker compose run --build --rm game`

### Mäta AI-modellerna

```
cd src/DungeonCrawler
dotnet run -- --benchmark gemma3:4b llama3.2:3b
```

Varje modell beskriver alla rum och svarar Merlin på en fråga. Resultatet skrivs som Markdown, med tid till första ordet och total tid. Modellerna måste vara nedladdade först, till exempel `docker exec ollama ollama pull llama3.2:3b`. Senaste mätningen finns i [docs/model-comparison.md](docs/model-comparison.md).

### Lägga till en ny figur

I `Game.cs`, metoden `CreateWorld()`:

```csharp
throneRoom.Characters.Add(new Character("Galahad",
    "Du är riddaren Galahad, ung, ärlig och lite högtidlig. Du har tappat din medaljong."));
```

Texten är instruktionen till AI:n: vem figuren är, hur den pratar och vad den vet.

### Begränsningar

Uppmätt på en bärbar dator utan grafikkort (Intel Core i5, 32 GB RAM). Fem modeller testades, se [docs/model-comparison.md](docs/model-comparison.md):

| Modell | Storlek | Tid till första ordet | Svenska |
|---|---|---|---|
| `gemma3:4b` (standard) | 3,4 GB | ca 6,5 s med laddare, ca 13 s på batteri | Klart bäst, naturlig och korrekt |
| `llama3.2:3b` | 2 GB | ca 2,5 s | Okej, men konstiga ord ibland |

- **Språk mot hastighet:** den modell som skriver bäst svenska är också två till fem gånger långsammare. På en dator med grafikkort skulle den vara mycket snabbare.
- AI:n kan hitta på detaljer som inte finns i spelet (så kallade *hallucinationer*). Därför bestämmer koden vilka föremål och utgångar som finns, och de visas alltid av koden, inte av AI:n.
- Svaren blir lite olika varje gång.

### Vidareutveckling

- Hämta nästa rums beskrivning i förväg medan spelaren läser, så att väntan försvinner helt
- Låta spelet välja modell automatiskt efter hur snabb datorn är
- Låta figurerna minnas tidigare samtal, även efter att man gått därifrån
- Fler figurer, till exempel Galahad i Tronsalen
- Automatiska tester av spelreglerna (rörelse, föremål, vinst) som körs utan AI:n

---

## Licens

MIT, se [LICENSE](src/DungeonCrawler/LICENSE).
