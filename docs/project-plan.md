# Projektplan: AI-spelledare i Jakten på den Heliga Graalen

## Vad är nytt

Jag ska lära mig att köra en **lokal språkmodell (AI)** med **Ollama i Docker** och koppla ihop den med ett eget program via ett **REST-API**.

Utbildningen har inte tagit upp hur man kör en AI-modell själv, eller hur ett program pratar med en AI-tjänst. Jag valde det för att AI-tjänster blir en allt vanligare del av system som ska kopplas ihop, och för att det går att göra gratis på min egen dator. Inga API-nycklar, inga kostnader och ingen data som lämnar datorn.

---

## Vad jag bygger

Jag bygger vidare på mitt gamla textäventyr **Jakten på den Heliga Graalen** (C#, .NET 9), från OOP-kursen (repot `assignment-oop-95hanmel`). Där spelar man riddaren Tristan och letar efter Graalen i tio rum i Camelot.

Spelet får en **AI-spelledare**:

- När Tristan går in i ett rum skickar spelet rummets namn, föremål och utgångar till AI:n. AI:n skriver en levande beskrivning i stället för den fasta texten.
- Ett nytt kommando, `prata med [person]`, där man kan prata med en figur, till exempel Merlin i Merlins Torn. AI:n svarar i rollen.
- Spelreglerna (rum, föremål, vinst) ligger kvar i C#-koden. AI:n bestämmer bara över texten, inte över vad som händer i spelet.
- Körs inte Ollama, eller tar svaret för lång tid, visar spelet den gamla fasta texten. Spelet går alltså alltid att spela.

Så hänger delarna ihop:

```
[Spelet i C#]  --HTTP/JSON-->  [Ollama i Docker]  -->  [Språkmodell, t.ex. llama3.2]
     ^                                                         |
     +--------------------- textsvar --------------------------+
```

**Minsta version som räknas som klar:**
1. Rumsbeskrivningarna skrivs av AI:n som körs lokalt i Docker.
2. Spelet fungerar ändå, med de gamla texterna, när AI:n inte är igång.

---

## Plan, vecka för vecka

| Kursvecka | Vad jag gör |
|---|---|
| 1 (v. 46) | Väljer idé, läser kursens instruktioner, hittar mitt gamla spel |
| 2 (v. 47) | Skriver projektplanen, skapar repot och lägger in spelets kod. Startar Ollama i Docker Desktop och testar en modell i terminalen. Lämnar in planen torsdag 19 november |
| 3 (v. 48) | Lär mig Ollamas REST-API. Skriver en liten C#-klass (`AiGameMaster`) som skickar en fråga och tar emot ett svar |
| 4 (v. 49) | Kopplar in AI:n i rumsbeskrivningarna. Bygger reservläget med de gamla texterna, och en tidsgräns om svaret dröjer |
| 5 (v. 50) | Lägger till kommandot `prata med`, med Merlin först. Finslipar instruktionerna till AI:n (prompten) så att den håller sig till spelets värld |
| 6 (v. 51) | Lägger till `docker-compose.yml` så att allt startar med ett kommando. Skriver README med en del för användare och en för utvecklare. Mäter hur snabba svaren är |
| Uppehåll | Vilar. Kanske läser jag om hur AI används i systemintegration, till rapporten |
| 7 (v. 2) | Skriver rapporten och gör slutinlämningen 14 januari |
| 8 (v. 3) | Presentation med demo och opposition, 19 eller 21 januari |

Loggboken skriver jag varje vecka i `docs/`, i Obsidian.

**Största risken, och vad jag gör då:** Att min dator är för långsam och att AI:n tar många sekunder på sig att svara. Då byter jag till en mindre modell (till exempel `llama3.2:1b`), gör beskrivningarna kortare eller låter spelet hämta texten i förväg. Fungerar det ändå inte har spelet sitt reservläge med de fasta texterna, och då visar jag AI-delen i en kortare demo.

---

## Ändringar

| Datum | Vad jag ändrade | Varför |
|---|---|---|
| | | |
