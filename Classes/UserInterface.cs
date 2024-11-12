using Figgle;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParkingApp.Classes
{
    public class UserInterface
    {
        private ParkingManager parkingManager;

        public UserInterface(ParkingManager parkingManager)
        {
            this.parkingManager = parkingManager;
        }
        public void StartApplication()
        {
            printFiggleBanner();

            while (true)
            {
                var options = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Navigera och välj ditt val i menyn!")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                        .AddChoices(new[] {
                        "Starta parkering", "Avsluta parkering", "Visa historik",
                        "Notifikationer", "Avsluta & spara",
                }));

                switch (options)
                {
                    case "Starta parkering":
                        parkingManager.StartParking();
                        break;

                    case "Avsluta parkering":
                        parkingManager.EndParking();
                        break;

                    case "Visa historik":
                        ShowHistoryOption();
                        break;

                    case "Notifikationer":
                        //Logik
                        break;

                    case "Avsluta & spara":
                        LoadProgressBar("Sparar data till databasen...");
                        SaveDataAndExit();
                        return;
                    default:
                        AnsiConsole.MarkupLine("[bold red]Ogiltligt val[/], försök igen!");
                        break;
                }
            }
        }
        public void ShowHistoryOption()
        {
            while(true)
            {
                // Ask for the user's favorite fruit
                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Navigera och välj ditt val i menyn!")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                        .AddChoices(new[] {
                        "Pågående parkeringar", "Avslutade parkeringar", "Tillbaka",
                }));

                switch(option)
                {
                    case "Pågående parkeringar":
                        parkingManager.ShowParkings(parkingManager.activeParkings, "Aktiva parkeringar");
                        break;
                    case "Avslutade parkeringar":
                        parkingManager.ShowParkings(parkingManager.parkingHistory, "Parkering historik", isHistory: true);
                        break;
                    case "Tillbaka":
                        return;
                    default:
                        AnsiConsole.MarkupLine("[bold red]Ogiltligt val[/], försök igen");
                        break;
                }
            }
        }
        private async void SaveDataAndExit()
        {
            string filePath = "data.json";
            // Spara data till JSON via DataJson
            DataJson.SaveToFile(filePath, parkingManager.activeParkings, parkingManager.parkingHistory);
            //Console.Clear();
            AnsiConsole.MarkupLine("[bold green]Data sparad. Avslutar programmet...[/]");
        }
        private static void LoadProgressBar(string taskMessage)
        {
            // Synchronous
            AnsiConsole.Progress()
                .Start(ctx =>
                {
                    // Define tasks
                    var task1 = ctx.AddTask($"[green]{taskMessage}[/]");

                    while (!ctx.IsFinished)
                    {

                        task1.Increment(2);
                        Task.Delay(100).Wait();
                    }
                });
        }
        public static void printFiggleBanner()
        {
            string figgleTitle = FiggleFonts.Standard.Render("Parkeringsapp");
            AnsiConsole.MarkupLine($"[bold blue]{figgleTitle}[/]\n");
        }
    }
}
