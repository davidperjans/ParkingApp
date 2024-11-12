using Newtonsoft.Json;
using Spectre.Console;
using Figgle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingApp.Classes
{
    public class ParkingManager
    {//hej
        public Dictionary<Guid, ParkingEvent<Guid>> activeParkings { get; private set; } = new();
        public Dictionary<Guid, ParkingEvent<Guid>> parkingHistory { get; private set; } = new();

        private ParkingSimulator simulator;

        public ParkingManager(ParkingSimulator simulator, Dictionary<Guid, ParkingEvent<Guid>> loadedActiveParkings, Dictionary<Guid, ParkingEvent<Guid>> loadedParkingHistory)
        {
            this.simulator = simulator;
            this.activeParkings = loadedActiveParkings ?? new Dictionary<Guid, ParkingEvent<Guid>>();
            this.parkingHistory = loadedParkingHistory ?? new Dictionary<Guid, ParkingEvent<Guid>>();
        }

        public Guid StartParking(string regNr)
        {
            var id = Guid.NewGuid();
            var parkingEvent = new ParkingEvent<Guid>(id, simulator.GetSimulatedTime(), regNr);
            activeParkings[parkingEvent.Id] = parkingEvent;
            Console.WriteLine($"Parkeringen {id} startades vid {parkingEvent.StartTime} för bil med regnr: {regNr}");
            Console.WriteLine($"Aktiva parkeringar: {activeParkings.Count}"); // Kontrollera antalet aktiva parkeringar
            return id;
        }

        public void EndParking(string regNr, decimal hourlyRate)
        {
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
                Console.WriteLine($"Ingen aktiv parkering hittades med ID: {regNr}.");
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
    }
}
