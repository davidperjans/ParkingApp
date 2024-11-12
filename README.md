# 🚗 Parkeringsapp

En modern och enkel konsolapplikation för att hantera parkeringar! Med Parkeringsappen kan du:

- Starta och avsluta parkeringar ⏱️
- Se en fullständig historik över alla parkeringar 📜
- Spara data säkert i JSON för att hålla koll på parkeringshistorik mellan sessioner 💾

![Parkering App Banner](link_till_din_banner_bild.gif) <!-- Banner-bild eller GIF här -->

---

## 💬 Framtida förbättringar

- **Notifikationer**: Lägg till funktion som skickar notiser när din parkering snart går ut 📲
- **Ställa in tid direkt**: Göra det möjligt för användare att ställa in bestämd tid från start av parkeringen ⏰
- **UI-förbättringar**: Använda mer avancerade konsoleffekter 🎨

---

## ✨ Funktioner

- 🚗 **Starta och avsluta parkeringar**: Registrera parkeringar med start- och sluttider samt beräkna kostnaden automatiskt.
- 📜 **Historikvisning**: Se både aktiva och avslutade parkeringar i en stilren tabell.
- 💾 **JSON-lagring**: Spara parkeringarna till en JSON-fil så att du aldrig tappar data.
- ✅ **Validering**: Säkerställer att varje registreringsnummer följer formatet **ABC123** (3 bokstäver följt av 3 siffror).

---

## 🧠 Lärdomar

### JSON-hantering
Jag lärde mig att arbeta med JSON för att spara och ladda parkeringar. Med `Newtonsoft.Json` kan appen enkelt spara data i JSON och ladda tillbaka den i samma format för framtida sessioner. **Effektivt och pålitligt!** 📂

### Generiska Klasser och Metoder
Jag använde generiska klasser och metoder för att hantera både pågående och avslutade parkeringar på ett strukturerat och återanvändbart sätt. Genom generiska metoder som `ShowParkings` kan vi skapa en enhetlig visning för olika typer av data:

```csharp
// Generisk metod för att visa parkeringar
public void ShowParkings<T>(Dictionary<Guid, T> parkings, bool isHistory = false)
{
    // Metodens innehåll...
}
```
## 🔍 Validering av Registreringsnummer
En särskild metod ser till att alla registreringsnummer följer formatet ABC123. Detta gör att datan håller hög kvalitet och att användaren alltid följer ett korrekt format.

## 🛠️ Teknisk Översikt
<!-- Klassdiagram eller flödesschema -->

### Klassstruktur

- **UserInterface**: Hanterar användarinteraktion och menyer i konsolen.
- **ParkingManager**: Ansvarar för affärslogik, såsom att starta och avsluta parkeringar, samt hantera data i JSON-format.
- **ParkingEvent<T>**: En generisk klass som representerar varje parkering, innehållande starttid, sluttid och beräknad kostnad.

### Viktiga Metoder

- `StartParking(string regNr)`: Startar en ny parkering baserat på ett registreringsnummer som användaren matar in.
- `EndParking(Guid parkingId)`: Avslutar en parkering och beräknar kostnaden baserat på tid genom att jämföra start- och sluttid.
- `ShowParkings<T>(Dictionary<Guid, ParkingEvent<Guid>> parkings)`: En generisk metod som visar parkeringar i tabellformat och fungerar för både pågående och avslutade parkeringar.

## 📂 JSON-lagringsexempel

JSON-filen som sparar parkeringdata kan se ut så här:

```json
[
  {
    "Id": "a unique GUID",
    "StartTime": "2023-05-01T08:00:00",
    "EndTime": "2023-05-01T12:00:00",
    "Cost": 120.0,
    "RegNr": "ABC123"
  }
]
