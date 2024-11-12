using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingApp.Classes
{
    public class ParkingSimulator
    {
        private DateTime simulatedTime;

        public ParkingSimulator()
        {
            simulatedTime = DateTime.Now;
        }

        public DateTime GetSimulatedTime()
        {
            return simulatedTime;
        }

        public void AdvanceTime(TimeSpan timeSpan)
        {
            simulatedTime = simulatedTime.Add(timeSpan);
            Console.WriteLine($"Simulerad tid nu: {simulatedTime}");
        }

    }
}
