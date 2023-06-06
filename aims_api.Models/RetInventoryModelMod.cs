using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class RetInventoryModelMod
    {
        public string? ReturnsId { get; set; }
        public string? ReturnsLineId { get; set; }
        public int QtyToReceive { get; set; }
        public string? UserAccountId { get; set; }
    }
}
