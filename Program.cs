using ParkingApp.Classes;

namespace ParkingApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = "data.json";
            ParkingSimulator simulator = new ParkingSimulator();

            // Ladda tidigare parkeringar från fil genom DataJson
            var (loadedActiveParkings, loadedParkingHistory) = DataJson.LoadFromFile(filePath);

            // Skapa en ParkingManager med den laddade datan
            var parkingManager = new ParkingManager(simulator, loadedActiveParkings, loadedParkingHistory);

            UserInterface ui = new UserInterface(parkingManager);

            ui.StartApplication();
        }
    }
}
