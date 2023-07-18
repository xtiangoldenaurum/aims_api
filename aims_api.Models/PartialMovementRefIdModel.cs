using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class PartialMovementRefIdModel
    {
        public string? InventoryId { get; set; }
        public string? ReceivingId { get; set; }
        public string? MovementTaskId { get; set; }
        public string? UserAccountId { get; set; }
    }
}
