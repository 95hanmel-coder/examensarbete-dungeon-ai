# Loggbok – Examensarbete: AI-spelledare i Jakten på den Heliga Graalen

Repo: https://github.com/95hanmel-coder/examensarbete-dungeon-ai

## 2026-11-17 (kursvecka 2, v. 47)

- **Gjort:**
  - Läste kursens instruktioner och valde idé: mitt gamla textäventyr från OOP-kursen får en AI-spelledare som körs lokalt med Ollama i Docker.
  - Hittade det gamla spelet i en zip-fil. Det låg inte på mitt GitHub-konto utan i skolans organisation (Campus-Molndal-CLO25).
  - Skrev projektplanen (`docs/project-plan.md`) och skapade repot på GitHub.
  - Flyttade in spelets kod i `src/DungeonCrawler`.
  - Startade Ollama i Docker Desktop och laddade ner två modeller: `llama3.2:1b` (1,3 GB) och `llama3.2:3b` (2 GB).
  - Skrev klassen `AiGameMaster` som skickar rummets namn, beskrivning och föremål till Ollamas REST-API och får tillbaka en beskrivning.
  - Lade till ett reservläge: om Ollama inte är igång visas de gamla texterna, så spelet går alltid att spela. Testat.
  - Lade till kommandot `prata med [namn]` och två figurer som AI:n spelar: Merlin i Merlins Torn och Nimue i Nimues Spegelsal. Figuren minns vad man sagt under samtalet.
  - Testspelade hela vägen till Merlin. Han svarade: "Norr om ett ställe som ligger nära en källa" – en korrekt ledtråd, eftersom Graalen ligger norr om borggården med fontänen.
  - Problem när jag själv testspelade: jag skrev `syd` och `väst`, men spelet svarade "Förstår inte kommandot". Spelet krävde `gå syd`, fast rummet skriver "Det går att gå syd". Dessutom stod det "öster, väster" i hjälptexten, men spelet förstod bara "öst, väst" (en gammal bugg från OOP-kursen). Löst: nu räcker det att skriva bara riktningen, och flera stavningar fungerar (norr/nord/n, syd/söder/s, öst/öster/ö, väst/väster/v).
  - Problem 2 vid testspelning: riktningarna kändes inte fasta. Från Drakgrottan gick väst till Tronsalen, men från Tronsalen kunde man inte gå tillbaka – öst ledde till Lancelots Kammare. Jag jämförde koden med min gamla karta (`Tristans karta.png`): nio av tio vägar stämde åt båda hållen, men genvägen Drakgrottan → Tronsalen var enkelriktad (en pil på kartan). Löst: lade till `syd` från Tronsalen tillbaka till Drakgrottan. Testat åt båda hållen.
  - Problem 3 vid en lång testspelning (hela vägen till Merlin):
    - `ta brev` fungerade inte, man var tvungen att skriva `ta brev med sigill`. Löst: det räcker nu med början av namnet.
    - AI:n skrev 3–4 meningar fast jag bad om två. Löst: koden klipper texten efter två meningar (`KeepSentences`).
    - AI:n hittade på en fackla i Runda Bordets Sal. Orsak: exemplet i min instruktion handlade om Kapellet med en fackla, och AI:n kopierade detaljer därifrån till andra rum. Löst: exemplet är nu ett påhittat rum (Vaktstugan med en kniv) som inte finns i spelet.
    - Efter ändringarna: korta beskrivningar och inga påhittade föremål i testet.
    - Kvar att fundera på: Merlin pratar ibland filosofiskt i stället för att ge ledtrådar, och skriver "grål" i stället för "graal".
  - Lade till `docker-compose.yml` och en `Dockerfile` för spelet. Nu startar allt med ett kommando: `docker compose run --rm game`. Compose startar tre delar: Ollama (AI-tjänsten), en som laddar ner modellen, och spelet. Spelet når AI:n på adressen `http://ollama:11434` i Dockers eget nätverk. Testat: spelet körs i Docker och AI:n svarar.
  - Problem: spelet kraschade i slutet (felkod) när det kördes utan tangentbord, t.ex. i Docker eller i mina tester, eftersom det väntade på en tangenttryckning. Löst: spelet väntar bara på tangent när någon faktiskt spelar.
  - Skrev README med en del för användare (starta, kommandon, felsökning) och en för utvecklare (arkitektur, struktur, inställningar, begränsningar, vidareutveckling).
  - Förbättrade spelet: texten visas nu ord för ord medan AI:n skriver (streaming), figurerna vet vad Tristan bär på, en ny figur (Draken i Drakgrottan, grinig och sömnig), och `titta` använder också AI:n. Tog bort alla kompilatorvarningar.
  - Byggde ett mätläge i spelet: `dotnet run -- --benchmark [modeller]`. Det ber varje modell beskriva alla tio rummen och svara Merlin, och mäter tid till första ordet och total tid. Resultatet sparas i `docs/model-comparison.md`.
  - Jämförde fem modeller: llama3.2:1b, llama3.2:3b, gemma2:2b, qwen2.5:3b och gemma3:4b.
  - Problem: datorn gick i viloläge mitt i mätningen (21:17–23:01), så ett rum fick tiden 6 267 s. Efteråt gick den på batteri, vilket gjorde gemma3:4b långsammare (ca 13 s till första ordet mot 6,5 s med laddare). Jag hittade det i Windows händelselogg.
  - Mätningen avslöjade en bugg: tidsgränsen på 60 s gällde bara tills AI:n började svara. Om den stannade mitt i ett svar kunde spelet fastna för alltid. Löst med en tidsgräns för hela svaret (`CancellationTokenSource`).
  - Stopp på vägen: spelet krävde .NET 9, men datorn har bara .NET 8 och 10.
- **Lärt mig:**
  - Ett program kan prata med en AI som en vanlig webbtjänst: man skickar JSON med HTTP till `http://localhost:11434/api/chat` och får JSON tillbaka. Det är systemintegration i liten skala.
  - Jämförelse av modellerna med samma fråga:

    | | `llama3.2:1b` | `llama3.2:3b` |
    |---|---|---|
    | Svarstid | ca 4 s | ca 3 s (11 s första gången, när modellen laddas in i minnet) |
    | Svenska | Blandar in engelska och hittar på ord ("somjölk") | Bättre, men fortfarande konstiga ord ibland |
    | Följer instruktioner | Dåligt | Hittar på saker, t.ex. att Tristan "har tagit" en trolldryck |

  - AI:n hittar på saker som inte hänt, det kallas *hallucinationer*. Det blev bättre när jag gav den tydliga regler, ett exempel på ett bra svar och en lägre "temperature" (0,6), som gör den mindre påhittig. Den skriver ändå ofta fler meningar än jag ber om.
  - Viktigt designbeslut: spelets regler (rum, föremål, vinst) ligger kvar i C#-koden. AI:n bestämmer bara över texten, så den kan inte förstöra spelet.
  - Exempel i en AI-instruktion styr mer än man tror: AI:n kopierar detaljer från exemplet. Exemplet ska därför vara något som inte finns i spelet. Att inte lita blint på AI:n utan kontrollera svaret i koden (som att klippa efter två meningar) fungerar bättre än att bara be snällt.
  - Docker Compose: flera containrar kan prata med varandra via namn (`ollama`) i stället för IP-adresser. `depends_on` med villkor styr ordningen: spelet startar först när modellen är nedladdad, och modellen laddas ner först när Ollama är redo (healthcheck). Det är samma idé som när tjänster kopplas ihop i större system.
  - Resultat av jämförelsen:

    | Modell | Första ordet | Svenska | Följer instruktioner |
    |---|---|---|---|
    | llama3.2:1b | 2,2 s | Dålig | Nej, upprepar bara instruktionen |
    | gemma2:2b | 4,4 s | Ganska bra | Hittar på ("Drakfjället i din hand") |
    | qwen2.5:3b | 2,1 s | Många felstavningar | Merlin avslöjar svaret rakt ut |
    | llama3.2:3b | 2,5 s | Okej | Ganska bra |
    | gemma3:4b | 6,5 s (laddare) / 13 s (batteri) | Klart bäst | Bra, Merlin kommenterar facklan Tristan bär |

  - Det finns en avvägning mellan kvalitet och hastighet. En större modell skriver bättre men tar längre tid på en dator utan grafikkort. Streaming gör väntan lättare att stå ut med, eftersom man ser att något händer.
  - Mätningar påverkas av saker utanför koden (viloläge, batteri). Man måste skriva ner under vilka förhållanden man mätt.
  - Att testa själv som en riktig spelare hittar andra fel än automatiska tester. Användbarhet räknas för VG ("lätt att använda").
  - Merlin ger bra gåtfulla ledtrådar ("Den som vill komma till det blåga, måste först komma till det gröna") utan att avslöja var Graalen är.
- **Beslut:**
  - Bytte från .NET 9 till .NET 10, eftersom datorn bara har .NET 8 och 10 och .NET 10 har längre support (LTS). Skrivet i planens ändringstabell.
  - Valde först `llama3.2:3b` som standardmodell, eftersom svenskan blev bättre än i 1b och den var lika snabb.
  - Bytte sedan till `gemma3:4b` som standard efter jämförelsen av fem modeller: klart bäst svenska, och med streaming känns väntan rimlig. `llama3.2:3b` finns kvar som snabbt alternativ (`OLLAMA_MODEL`). Tog bort de tre sämsta modellerna (llama3.2:1b, gemma2:2b, qwen2.5:3b) för att spara cirka 5 GB.
  - Gjorde vecka 3–6 i planen (API-anrop, reservläge, `prata med`, docker-compose, README) tidigare än planerat. Det gick snabbare än jag trodde. Den tid jag vinner lägger jag på att förbättra AI:ns svar och testa med andra personer.
- **Nästa:**
  - Få AI:n att hålla sig till två meningar och bättre svenska. Kanske testa en modell som är bättre på svenska.
  - Låta någon annan följa README:n och spela, för att se om den är lätt att förstå.
  - Göra om mätningen av gemma3:4b med laddaren i och utan viloläge, för säkrare siffror.
  - Kanske hämta nästa rums beskrivning i förväg, så att väntan försvinner helt.
