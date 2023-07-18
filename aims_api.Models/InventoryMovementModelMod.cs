using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class InventoryMovementModelMod
    {
        public string? InvMoveId { get; set; }
        public string? InvMoveLineId { get; set; }
        public int QtyToMove { get; set; }
        public string? UserAccountId { get; set; }
    }
}
