using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class InventoryModelMod
    {
        public string? POId { get; set; }
        public string? POLineId { get; set; }
        public int QtyToReceive { get; set; }
        public string? UserAccountId { get; set; }
    }
}
