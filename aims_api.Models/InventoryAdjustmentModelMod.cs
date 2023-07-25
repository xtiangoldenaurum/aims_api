using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class InventoryAdjustmentModelMod
    {
        public string? InvAdjustId { get; set; }
        public string? InvAdjustLineId { get; set; }
        public int QtyToAdjust { get; set; }
        public string? UserAccountId { get; set; }
    }
}
