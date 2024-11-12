using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ParkingApp.Classes
{
    public class DataJson
    {
        // Spara data till en JSON-fil
        public static void SaveToFile(string filePath, Dictionary<Guid, ParkingEvent<Guid>> activeParkings, Dictionary<Guid, ParkingEvent<Guid>> parkingHistory)
        {
            var data = new
            {
                activeParkings = activeParkings.Values,
                parkingHistory = parkingHistory.Values
            };

            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Console.WriteLine("Data saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        // Ladda data från JSON-fil
        public static (Dictionary<Guid, ParkingEvent<Guid>> activeParkings, Dictionary<Guid, ParkingEvent<Guid>> parkingHistory) LoadFromFile(string filePath)
        {
            var activeParkings = new Dictionary<Guid, ParkingEvent<Guid>>();
            var parkingHistory = new Dictionary<Guid, ParkingEvent<Guid>>();

            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var data = JsonConvert.DeserializeObject<dynamic>(json);

                    // Ladda och konvertera från JSON till Dictionaries
                    foreach (var item in data.activeParkings)
                    {
                        var parkingEvent = JsonConvert.DeserializeObject<ParkingEvent<Guid>>(item.ToString());
                        activeParkings[parkingEvent.Id] = parkingEvent;
                    }

                    foreach (var item in data.parkingHistory)
                    {
                        var parkingEvent = JsonConvert.DeserializeObject<ParkingEvent<Guid>>(item.ToString());
                        parkingHistory[parkingEvent.Id] = parkingEvent;
                    }

                    Console.WriteLine("Data loaded successfully.");
                }
                else
                {
                    Console.WriteLine("No previous data found, starting fresh.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }

            return (activeParkings, parkingHistory);
        }
    }
}
