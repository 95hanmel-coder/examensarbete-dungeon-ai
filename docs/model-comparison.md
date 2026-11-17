# Jämförelse av AI-modeller (2026-11-17)

Samma instruktioner som i spelet. Tid till första ordet = hur länge spelaren väntar innan texten börjar synas.

Körd med `dotnet run -- --benchmark llama3.2:1b llama3.2:3b gemma2:2b qwen2.5:3b gemma3:4b` på en bärbar dator utan grafikkort (Intel Core i5-1335U, 32 GB RAM). AI:n körs alltså på processorn.

## Sammanfattning

| Modell | Storlek | Första ordet (medel) | Svenska | Följer instruktionerna | Beslut |
|---|---|---|---|---|---|
| llama3.2:1b | 1,3 GB | 2,2 s | Dålig | Nej, upprepar bara instruktionen ("Rum: … Grundbeskrivning: …") | Borttagen |
| gemma2:2b | 1,6 GB | 4,4 s | Ganska bra | Hittar på: "Drakfjället i din hand", "På din hjälm" | Borttagen |
| qwen2.5:3b | 1,9 GB | 2,1 s | Många felstavningar ("Draffjällen", "skjalar") | Merlin avslöjar svaret rakt ut, trots att han bara ska ge ledtrådar | Borttagen |
| llama3.2:3b | 2,0 GB | 2,5 s | Okej, men konstiga ord ibland ("sittar", "föll") | Ganska bra | **Kvar, snabbt alternativ** |
| gemma3:4b | 3,4 GB | 12,8 s* | **Klart bäst**, naturlig och korrekt | Bra. Merlin kommenterar facklan Tristan bär och ger en ledtråd utan att avslöja | **Kvar, standard** |

\* Mätningen av gemma3:4b störs av två saker: datorn gick i viloläge mitt i (därför 6 267 s för Lancelots Kammare och det höga medlet "totalt"), och efteråt gick den på batteri, vilket gör processorn långsammare. Före viloläget, med laddare, tog första ordet 6,5 s.

**Slutsats:** Det finns en avvägning mellan språk och hastighet. gemma3:4b skriver mycket bättre svenska men är två till fem gånger långsammare. Eftersom texten visas medan den skrivs (streaming) märks väntan mindre, så jag valde gemma3:4b som standard. Den som har en långsam dator kan byta till llama3.2:3b med `OLLAMA_MODEL`.

---

## llama3.2:1b

Kallstart (modellen laddas in i minnet): 6,8 s

| Rum | Första ordet | Totalt | Beskrivning |
|---|---|---|---|
| Runda Bordets Sal | 2,0 s | 3,6 s | Rum: Runda Bordets Sal. Grundbeskrivning: Riddarna sittar i en runda sal, där de bär sig om diskussionen om Graalen. |
| Lancelots Kammare | 1,4 s | 2,5 s | Rum: Lancelots Kammare. Grundbeskrivning: Elegant rum med säng och vapensköldar. |
| Tronsalen | 1,6 s | 3,1 s | Rum: Tronsalen. Du kliver in i storslagen tronsalen, där hela sal är dekorerad med blomster och målningar. |
| Kungliga Kammaren | 2,7 s | 4,4 s | Rum: Kungliga Kammaren. Grundbeskrivning: Luxurierad inredning med målverk av de mest höga kända artisterna. |
| Kapellet | 1,9 s | 2,9 s | Rum: Kapellet. Grundbeskrivning: Heligt rum med altare och statyer. |
| Drakgrottan | 2,8 s | 3,7 s | Rum: Drakgrottan. Grundbeskrivning: Mörk grotta. |
| Camelots Borggård | 2,7 s | 3,9 s | Rum: Camelots Borggård. Grundbeskrivning: En öppen gård med fontän och blommor. |
| Avalons Trädgård | 2,7 s | 4,2 s | Rum: Avalons Trädgård. Grundbeskrivning: Lummig trädgård med exotiska växter. |
| Merlins Torn | 1,4 s | 2,7 s | Rum: Merlins Torn. Grundbeskrivning: Mystiskt torn med böcker och trolldrycker. |
| Nimues Spegelsal | 2,9 s | 4,2 s | Rum: Nimues Spegelsal. Du hör en stilla konsentrering, som kommer från mitt mästerverk. |

**Medel:** första ordet 2,2 s, totalt 3,5 s

**Merlin** (6,9 s): "Var finns Heliga Graalen?"

> Har du hört berättelsen om Avalons Trädgård? Ja, det rymmer något om en bergsberg som sträcker sig ner i skogen, norr om Camelots Borggård. Det heter... (vad heter det sitt?

## llama3.2:3b

Kallstart (modellen laddas in i minnet): 15,1 s

| Rum | Första ordet | Totalt | Beskrivning |
|---|---|---|---|
| Runda Bordets Sal | 1,3 s | 5,6 s | Du står nu i Runda Bordets Sal, där en mängd riddar sittar vid det stora runda bordet. Diskussioner om Graalen pågår, och riddarna rör sig uppmuntrade i sina diskussioner. |
| Lancelots Kammare | 3,2 s | 8,3 s | Du kliver in i Lancelots elegant kammare, där solsken strålar in genom fönstret och föll på det vackra vapensköld som ligger på sängen. På bordet ligger ett brev med sigill, som glimmar i det solskenet. |
| Tronsalen | 1,0 s | 6,4 s | Du kliver in i Tronsalen, där ljuset från fönstren bryter mot den storslagna salen. Medelstora, pampiga skålar och prydnadsföremål står utställda, men det är en liten, glansfull medaljong som rör dig mest. |
| Kungliga Kammaren | 2,9 s | 7,7 s | Du står uppmärksam i den imponerande Kungliga Kammaren, där dyra möbler och tapeter omger dig. Merlins spådomsbok ligger öppen på bordet, som om den just skrivits av en vitskald hand. |
| Kapellet | 3,3 s | 6,9 s | Du kliver in i kapellets heliga rum, där statyer i brons står på altaren. Du ser en stor urholkning på det, där en del av altaret är borttagen. |
| Drakgrottan | 2,6 s | 6,0 s | Du träder in i den mörka drakgrottan, där luften är sval och fuktig. Du kan höra vatten rinnande från någon av grotts sidor. |
| Camelots Borggård | 3,0 s | 8,2 s | Du står på Camelots borggård, där solskenen faller på en öppen plats med en fontän som flödar och blommor som blomstrar runt omkring. Riddarhjälm ligger på en ställe, nästan skadad av tid. |
| Avalons Trädgård | 1,1 s | 5,4 s | Du kliver genom trädgårdens trånga väggar, där exotiska träd och buskar utstäcker sig i all riktning. Ljudet av vatten strömmar från en närliggande källa. |
| Merlins Torn | 3,5 s | 8,5 s | Du står på trappan till Merlins Torn, där det mörka rummet möter dig. Du ser en källare med böcker som står i rader, och ett skåp som ligger öppet, med trolldryck och en trollstav bland annat. |
| Nimues Spegelsal | 3,5 s | 8,2 s | Du kliver in i den mystiska salen, där du omgivs av speglar som reflekterar varje rörelse. Doften av syrén och kråsbär fyller luften och ger dig en känsla av mystisk ömhet. |

**Medel:** första ordet 2,5 s, totalt 7,1 s

**Merlin** (11,5 s): "Var finns Heliga Graalen?"

> "Vägen till Graalen ligger inte på en enkel väg, utan i mörkret ligger en skugga som kallar dig. Norr om Camelots höjder, där solen aldrig ser att förlora sin värme, och där vatten och jord möts i en mystisk syneri."

## gemma2:2b

Kallstart (modellen laddas in i minnet): 7,8 s

| Rum | Första ordet | Totalt | Beskrivning |
|---|---|---|---|
| Runda Bordets Sal | 4,1 s | 6,4 s | Du ser Riddarna i salen, alla med sina ansikten täckta av fokus. De sitter runt det stora runda bordet och diskuterar Graalen. |
| Lancelots Kammare | 5,0 s | 6,8 s | Du ser en säng i ena hörnet av rummet.  Förmånligt och elegant är det gjord av fina material. |
| Tronsalen | 5,7 s | 7,8 s | Du ser en stor, elegant tron i mitten av rummet. Den är gjord av svarta stenar och det glittrar i de blåa lamporna. |
| Kungliga Kammaren | 4,9 s | 7,1 s | Du ser en elegant inredning i rummet. Merlins spådomsbok ligger öppen på bordet och det är en ökad känsla av mystik i luften. |
| Kapellet | 4,7 s | 6,6 s | Du ser en fackla med lite metall i det höga rymmet. Altaret är öppet och visar en urholkning i det. |
| Drakgrottan | 3,9 s | 5,6 s | Du känner en viss tystnad i grotta.  Drakfjället i din hand är tyst och glatt mot skinnet. |
| Camelots Borggård | 3,8 s | 6,8 s | Du ser en stor fontän i mitten av gården, där blommor pryder gräsmattan runtomkring.  På din hjälm, en ridderhjälm, reflekterar det sken av solen. |
| Avalons Trädgård | 3,9 s | 6,4 s | Du ser en lummig trädgård med exotiska växter och en känsla av magisk välstånd.  Heliga Graalen står i mitten av trädgården, glödande i solen. |
| Merlins Torn | 4,0 s | 6,8 s | Du ser en mystisk atmosfär i Merlins Torn.  Möjlig stjärnskärmar glittrar i det mörka torn och en flask med en magisk bubblande substans. |
| Nimues Spegelsal | 4,2 s | 7,5 s | Du ser speglar i alla hjärtan av rummet, speglar som reflekterar ljuset. Doft av syrén och kråsbär flöder från en parfyymflaska som sitter på en liten bokhylla. |

**Medel:** första ordet 4,4 s, totalt 6,8 s

**Merlin** (5,8 s): "Var finns Heliga Graalen?"

> Graalen drömmer sig till en plats som är stilla och stilla, där ljuset inte når djupt.  Nordöst, där vinden sjunger en melodi av eld och vatten.

## qwen2.5:3b

Kallstart (modellen laddas in i minnet): 10,9 s

| Rum | Första ordet | Totalt | Beskrivning |
|---|---|---|---|
| Runda Bordets Sal | 3,3 s | 5,8 s | Du kliver in i runda bordets sal, där riddarna stillar sig vid dina inkomst. Graalen-toppet diskuseras med intresse. |
| Lancelots Kammare | 2,7 s | 5,7 s | Du kommer in i Lancelots kammar, där en elegant sköld väga mellan sängspaden. Brev med sigill ligger på bordet, men ingen annan föremål. |
| Tronsalen | 2,2 s | 4,7 s | Du kommer in i den stora tronsalen. Tronan står kraftigt i hörnet, avslutad med en glansande medaljong. |
| Kungliga Kammaren | 3,3 s | 7,0 s | Du kliver in i kunglig kammaren, där en stark eld tänds i hjälpbar brasa. Merlins spådomsbok, öppen på bordet, skjalar i eldskenet. |
| Kapellet | 2,1 s | 5,8 s | Du kliver in i kapellet, där ljus från en urholkning rör sig över altaret. Facklarna skymmar omkring hörnet och skenar i eldsskenerna. |
| Drakgrottan | 1,3 s | 5,1 s | Du går in i drakgrottan, där mörkhet tänder sig upp och du känner att din ansiktsuttryck visar försiktighet. Draffjällen ligger stilla och stilla på golvet. |
| Camelots Borggård | 1,1 s | 4,7 s | Du står på gränsen till Camelots Borggård, där fontänerna spela och blommorna växer livligt. Du känner sig uppskattad av sorgsfulla stillheten. |
| Avalons Trädgård | 1,2 s | 4,7 s | Du kliver in i Avalons Trädgård, en lummig trädgård med exotiska växter. Heliga Graalen ligger i en ädel rostad skål på bordet. |
| Merlins Torn | 1,5 s | 6,0 s | Du kliver in i merlins torn, där böckerna och trolldryck är samlade i en mystisk färgfuktig luft. På bordet ligger en trollstav och en flaska som rör sig i magiskt föremål. |
| Nimues Spegelsal | 2,7 s | 6,1 s | Du kliver in i Nimues Spegelsal, ett rum fyllt med glimmerande speglar och mystiskt luft. Du känner en känsla av vackra magi runt dig. |

**Medel:** första ordet 2,1 s, totalt 5,6 s

**Merlin** (7,0 s): "Var finns Heliga Graalen?"

> Heliga Graalen finns i Avalons Trädgård, norr om Camelots Borggård.

## gemma3:4b

Kallstart (modellen laddas in i minnet): 15,2 s

| Rum | Första ordet | Totalt | Beskrivning |
|---|---|---|---|
| Runda Bordets Sal | 6,5 s | 10,4 s | Du kliver in i den runda bordets sal, där ljud av diskussioner ekar. Framför dig ligger ett stort, runt bord med Excalibur på dess yta. |
| Lancelots Kammare | 6,5 s | 6 267,0 s | Du stiger in i Lancelots kammare, doften av rökelse hänger kvar i luften. En stor säng står mitt i rummet, och två vapensköldar vilar mot väggen. |
| Tronsalen | 13,2 s | 21,6 s | Du kliver in i den storslagna tronsalen, där en tron tronar i mitten av rummet. Pampiga dekorationer pryder väggarna och du känner en svag lukt av gammalt trä och liljor. |
| Kungliga Kammaren | 14,5 s | 22,5 s | Du kliver in i den kungliga kammaren, och en doft av gammalt pergament och liljekonvalj fyller luften. Öppet på bordet ligger ett dyrt brev, och bredvid det, Merlins spådomsbok. |
| Kapellet | 13,6 s | 20,2 s | Du kliver in i kapellet, doften av rökelse hänger tungt i luften. En fackla brinner på altaret och kastar dansande skuggor över de höga statyerna. |
| Drakgrottan | 13,8 s | 23,0 s | Du kliver in i den mörka grottan, och luften är fuktig och varm. Du känner en kall vind mot nacken och en obehaglig känsla av att du blir buret uppmärksam. |
| Camelots Borggård | 14,4 s | 20,3 s | Du kliver ut i Camelots borggård, där solen skiner starkt och värmer dig. En fontän sprutar vatten mot en blomsterbädd som doftar sött. |
| Avalons Trädgård | 13,1 s | 22,3 s | Du kliver in i Avalons trädgård, fylld med doften av främmande blommor. Solen silas genom de höga träden och träffar den Heliga Graalen. |
| Merlins Torn | 17,2 s | 24,8 s | Du står i Merlins torn, en doft av kryddor och gammal trä fyller luften. En flaska bubblar under ett svagt, violett sken och du känner en lätt värme från den. |
| Nimues Spegelsal | 15,1 s | 25,5 s | Du kliver in i Nimues Spegelsal, och du blir omedelbart omgiven av speglar i alla storlekar och former. Du känner doften av syrén och kråsbär, och ser en parfymflaska på ett smidigt bord. |

**Medel:** första ordet 12,8 s, totalt 645,7 s

**Merlin** (26,5 s): "Var finns Heliga Graalen?"

> Du bär en fackla, unge man, men ljuset avslöjar sällan det sanna. Tänk på att Avalon väntar norrut, där träden viskar hemligheter om den heliga skålen.  Följ rötternas slingor, och kanske finner du vad du söker.

