using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class StatsReport
    {
        public int? SpotsAvailable { get; set; }
        public double? TodaysRevenue { get; set; }
        public int? AverageCarsPerDay { get; set; }
        public double? AvgRevenueThirtyDays { get; set; }
    }
}
