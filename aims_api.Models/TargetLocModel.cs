using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class TargetLocModel
    {
        public string? ValidationCode { get; set; }
        public string? LocationId { get; set; }
        public string? LocationName { get; set; }
        public string? LPNTo { get; set; }
        public bool EnableLPNField { get; set; }
        public int OccupantQty { get; set; }
    }
}
