using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingApp.Classes
{
    public class ParkingData
    {
        public List<ParkingEvent<Guid>> activeParkings {  get; set; }
        public List<ParkingEvent<Guid>> parkingHistory { get; set; }
    }
}
