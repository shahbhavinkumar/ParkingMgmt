using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class ParkingInformation
    {
        public string TagNumber { get; set; } = default!;
        public DateTime InTime { get; set; }
        public DateTime? OutTime { get; set; }
        public double? Rate { get; set; }

        public double? AmountToCharge { get; set; } = 100.0;

        public int? HoursParked
        {
            get
            {
                return this.OutTime != null? Convert.ToInt32(Math.Ceiling((DateTime.Now - InTime).TotalHours)) : null;
               // return  Rate != null ? Convert.ToInt32(hours * Rate.Value) : 0;
            }            
        }
    }
}
