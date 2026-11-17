# Jakten på den Heliga Graalen – med AI-spelledare

Ett textäventyr i C# där du spelar riddaren Tristan och letar efter Heliga Graalen i Camelot. Spelet har en **AI-spelledare** som skriver rumsbeskrivningarna och spelar figurer som du kan prata med, till exempel trollkarlen Merlin.

AI:n körs **lokalt på din egen dator** med [Ollama](https://ollama.com) i Docker. Inget kostar pengar, inga konton eller API-nycklar behövs, och ingenting skickas ut på internet.

Examensarbete av Hanita Melin. Projektplanen finns i [docs/project-plan.md](docs/project-plan.md).

---

## För användare

### Det här behöver du

- [Docker Desktop](https://www.docker.com/products/docker-desktop/), som ska vara igång
- Cirka 4 GB ledigt diskutrymme för AI-modellen
- En dator med minst 8 GB RAM

### Starta spelet

1. Ladda ner projektet. Klicka på den gröna knappen **Code → Download ZIP** på GitHub och packa upp filen, eller använd git:
   ```
   git clone https://github.com/95hanmel-coder/examensarbete-dungeon-ai.git
   cd examensarbete-dungeon-ai
   ```
2. Öppna PowerShell eller en terminal i mappen och skriv:
   ```
   docker compose run --rm game
   ```
   Första gången tar det några minuter, eftersom AI-modellen (cirka 2 GB) laddas ner. Efter det startar spelet på några sekunder.

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
| Det första svaret tar lång tid | Helt normalt. AI-modellen laddas in i minnet första gången, det tar cirka 10 sekunder |
| AI:n skriver konstig svenska | Modellen är liten för att kunna köras på en vanlig dator. Se *Begränsningar* nedan |

---

## För utvecklare

### Så hänger delarna ihop

```
┌──────────────┐   HTTP POST /api/chat (JSON)   ┌──────────────┐      ┌──────────────┐
│  Spelet (C#) │ ─────────────────────────────► │    Ollama    │ ───► │ llama3.2:3b  │
│  container   │ ◄───────────────────────────── │  container   │      │ (AI-modell)  │
└──────────────┘          svar (JSON)           └──────────────┘      └──────────────┘
```

- **Spelets regler bestäms av C#-koden:** rum, utgångar, föremål och vinstvillkor. AI:n skriver bara text och kan inte ändra vad som händer i spelet.
- **Reservläge:** om Ollama inte svarar inom 30 sekunder, eller inte är igång, visar spelet de fasta texterna och slutar fråga AI:n.
- AI-svaren för rummen klipps efter två meningar i koden, eftersom modellen ofta skriver längre än den blir ombedd.

### Projektstruktur

```
examensarbete-dungeon-ai/
├── docker-compose.yml        # Startar Ollama, laddar ner modellen och kör spelet
├── docs/
│   └── project-plan.md       # Projektplan
└── src/DungeonCrawler/
    ├── Dockerfile            # Bygger spelet till en container
    ├── Program.cs            # Startpunkt
    ├── Game.cs               # Spelloop, kommandon, kartan
    ├── AiGameMaster.cs       # Pratar med Ollama: rumsbeskrivningar och samtal
    ├── Character.cs          # Figur som AI:n spelar (Merlin, Nimue)
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

Sätts som miljövariabler, till exempel i PowerShell: `$env:OLLAMA_MODEL = "llama3.2:1b"`

| Variabel | Standardvärde | Vad den gör |
|---|---|---|
| `OLLAMA_URL` | `http://localhost:11434` (i Docker: `http://ollama:11434`) | Adress till Ollama |
| `OLLAMA_MODEL` | `llama3.2:3b` | Vilken AI-modell som används |

Byta modell för hela lösningen: `$env:OLLAMA_MODEL = "llama3.2:1b"; docker compose run --rm game`

### Lägga till en ny figur

I `Game.cs`, metoden `CreateWorld()`:

```csharp
dragonCave.Characters.Add(new Character("Draken",
    "Du är en gammal, trött drake. Du pratar långsamt och vill bara sova."));
```

Texten är instruktionen till AI:n: vem figuren är, hur den pratar och vad den vet.

### Begränsningar

Uppmätt på min egen dator:

| Modell | Storlek | Svarstid | Svenska |
|---|---|---|---|
| `llama3.2:1b` | 1,3 GB | ca 4 s | Blandar in engelska ord och hittar på ord |
| `llama3.2:3b` (standard) | 2 GB | ca 3 s, ca 11 s första gången | Bättre, men ibland konstiga ord |

- AI:n kan hitta på detaljer som inte finns i spelet (så kallade *hallucinationer*). Därför bestämmer koden vilka föremål och utgångar som finns, och de visas alltid av koden, inte av AI:n.
- Svaren blir lite olika varje gång.

### Vidareutveckling

- Testa större eller mer svenskspråkiga modeller och jämföra kvalitet mot svarstid
- Visa AI:ns svar ord för ord medan det skrivs (streaming), så att väntan känns kortare
- Låta figurerna minnas vad Tristan bär på, så att Merlin kan kommentera trollstaven
- Fler figurer, till exempel draken i Drakgrottan

---

## Licens

MIT, se [LICENSE](src/DungeonCrawler/LICENSE).
