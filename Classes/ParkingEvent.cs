using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ParkingApp.Classes
{
    public class ParkingEvent<T>
    {
        public T Id { get; set; }
        public DateTime StartTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? EndTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Cost { get; set; }
        public string RegNr { get; set; }

        public ParkingEvent() { }

        public ParkingEvent(T id, DateTime startTime, string regNr)
        {
            Id = id;
            StartTime = startTime;
            EndTime = null;
            Cost = 0;
            RegNr = regNr;
        }
        public decimal CalculateCost()
        {
            if (EndTime.HasValue)
            {
                var totalHours = (EndTime.Value - StartTime).TotalHours;
                return (decimal)(totalHours * 30);
            }
            return 0;
        }
    }
}
