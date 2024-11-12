using Newtonsoft.Json;
using Spectre.Console;
using Figgle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ParkingApp.Classes
{
    public class ParkingManager
    {
        public Dictionary<Guid, ParkingEvent<Guid>> activeParkings { get; private set; } = new();
        public Dictionary<Guid, ParkingEvent<Guid>> parkingHistory { get; private set; } = new();

        private ParkingSimulator simulator;

        public int hourlyRate = 30;

        public ParkingManager(ParkingSimulator simulator, Dictionary<Guid, ParkingEvent<Guid>> loadedActiveParkings, Dictionary<Guid, ParkingEvent<Guid>> loadedParkingHistory)
        {
            this.simulator = simulator;
            this.activeParkings = loadedActiveParkings ?? new Dictionary<Guid, ParkingEvent<Guid>>();
            this.parkingHistory = loadedParkingHistory ?? new Dictionary<Guid, ParkingEvent<Guid>>();
        }

        public Guid StartParking()
        {
            var regNr = PromptForRegistrationNumber();
            var id = Guid.NewGuid();
            var parkingEvent = new ParkingEvent<Guid>(id, simulator.GetSimulatedTime(), regNr);
            activeParkings[parkingEvent.Id] = parkingEvent;
            AnsiConsole.MarkupLine($"[bold yellow]AVISERING:[/] Parkeringen [bold gray]{id}[/] startades vid {parkingEvent.StartTime} för bil med regnr: {regNr}");
            return id;
        }
        public void EndParking()
        {
            //Printa ut alla aktiva parkeringar först så användaren kan se vilka att avsluta.
            ShowParkings(activeParkings, "Aktiva parkeringar");

            //Fråga användaren efter regnummer att avsluta
            var regNr = PromptForRegistrationNumber();

            //Kontrollera att det angivna regnr är korrekt.
            var parkingEvent = activeParkings.Values.FirstOrDefault(parkering => parkering.RegNr.Equals(regNr, StringComparison.OrdinalIgnoreCase));

            if (parkingEvent != null)
            {
                DateTime endTime = simulator.GetSimulatedTime();
                TimeSpan duration = endTime - parkingEvent.StartTime;
                decimal cost = (decimal)duration.TotalHours * hourlyRate;

                //Avslutar parkeringen
                parkingEvent.EndTime = endTime;
                parkingEvent.Cost = cost;

                //Flyttar den från aktiv till historik
                parkingHistory[parkingEvent.Id] = parkingEvent;

                //Jag tar bort den från att vara aktiv
                activeParkings.Remove(parkingEvent.Id);

                NotifyParkingEnded(parkingEvent.Id, cost);
            }
            else
            {
                AnsiConsole.MarkupLine($"[bold red]Ingen aktiv parkering hittades med ID:[/] {regNr}.");
            }
        }
        public void ShowParkings<T>(Dictionary<Guid, T> parkings, string title, bool isHistory = false) where T : ParkingEvent<Guid>
        {
            // Skapa en table
            var table = new Table()
                .BorderColor(Color.Silver)
                .Border(TableBorder.Rounded);

            AnsiConsole.Live(table)
                .Start(ctx =>
                {
                    // Definiera kolumnnamn
                    table.AddColumn(new TableColumn("ID").Centered());
                    ctx.Refresh();
                    Thread.Sleep(250);
                    table.AddColumn(new TableColumn("Regnr").Centered());
                    ctx.Refresh();
                    Thread.Sleep(250);
                    table.AddColumn(new TableColumn("Start").Centered());
                    ctx.Refresh();
                    Thread.Sleep(250);

                    // Om historik, lägg till slut och kostnad
                    if (isHistory)
                    {
                        table.AddColumn(new TableColumn("Slut").Centered());
                        ctx.Refresh();
                        Thread.Sleep(250);
                        table.AddColumn(new TableColumn("Kostnad").Centered());
                        ctx.Refresh();
                        Thread.Sleep(250);
                    }

                    // Lägg till varje parkering i tabellen
                    foreach (var entry in parkings.Values)
                    {
                        // Definiera färg för kostnad om historik
                        var costColor = entry.Cost.HasValue && entry.Cost > 50 ? Color.Red : Color.Green;

                        // Lägg till rad beroende på om det är historik eller aktiv parkering
                        if (isHistory)
                        {
                            table.AddRow(
                                new Markup($"[bold yellow]{entry.Id}[/]"),
                                new Markup($"[bold yellow]{entry.RegNr}[/]"),
                                new Markup($"[bold yellow]{entry.StartTime:yyyy-MM-dd HH:mm}[/]"),
                                new Markup($"[bold yellow]{entry.EndTime?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"}[/]"),
                                new Markup(entry.Cost.HasValue ? $"[{costColor}]{entry.Cost:C}[/]" : "[gray]N/A[/]")
                            );
                        }
                        else
                        {
                            table.AddRow(
                                new Markup($"[bold yellow]{entry.Id}[/]"),
                                new Markup($"[bold yellow]{entry.RegNr}[/]"),
                                new Markup($"[bold yellow]{entry.StartTime:yyyy-MM-dd HH:mm}[/]")
                            );
                        }
                    }
                });

            // Vänta på användarens input
            AnsiConsole.WriteLine("\nTryck på en tangent för att fortsätta...");
            Console.ReadKey();
            Console.Clear();
            UserInterface.printFiggleBanner();
        }
        public void NotifyParkingEnded(Guid parkingId, decimal cost)
        {
            AnsiConsole.MarkupLine($"[bold yellow]AVISERING[/]: Parkering [bold gray]{parkingId}[/] har avslutats. Totalkostnad: {cost:C}.");
        }
        public string PromptForRegistrationNumber()
        {
            while (true)
            {
                var regNr = AnsiConsole.Prompt(new TextPrompt<string>("Ange bilens registreringsnummer [bold yellow](ABC123)[/]: "))?.Trim().ToUpper();

                if (IsValidRegistrationNumber(regNr!))
                {
                    return regNr!; // Returnerar det godkända registreringsnumret
                }
                else
                {
                    AnsiConsole.MarkupLine("[bold red]Ogiltigt registreringsnummer.[/] Ange i formatet [bold yellow]'ABC123'[/] (3 bokstäver följt av 3 siffror).");
                }
            }
        }
        public static bool IsValidRegistrationNumber(string regNr)
        {
            // Kontrollera att regNr inte är null och att det har rätt längd
            if (string.IsNullOrWhiteSpace(regNr) || regNr.Length != 6)
            {
                return false;
            }

            // Kontrollera mönstret: 3 bokstäver följt av ett mellanslag och 3 siffror
            string pattern = @"^[A-Z]{3}[0-9]{3}$";
            return Regex.IsMatch(regNr, pattern, RegexOptions.IgnoreCase);
        }

    }
}
