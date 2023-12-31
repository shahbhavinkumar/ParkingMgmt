﻿using System;
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

        public int? HoursParked
        {
            set { }
           
            get 
            {
                return Convert.ToInt32(Math.Ceiling((DateTime.Now - InTime).TotalHours));
            }
        }
    }
}
