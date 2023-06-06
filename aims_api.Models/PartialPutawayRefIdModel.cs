using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class PartialPutawayRefIdModel
    {
        public string? InventoryId { get; set; }
        public string? ReceivingId { get; set; }
        public string? PutawayTaskId { get; set; }
        public string? TargetTrackId { get; set; }
        public string? ProductConditionId { get; set; }
        public string? UserAccountId { get; set; }
    }
}
