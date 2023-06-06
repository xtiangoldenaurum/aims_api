using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class WhTransInventoryModelMod
    {
        public string? WhTransferId { get; set; }
        public string? WhTransferLineId { get; set; }
        public int QtyToReceive { get; set; }
        public string? UserAccountId { get; set; }
    }
}
