using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class RetReceivedDetailModel
    {
        public string? ReceivingId { get; set; }
        public string? InventoryId { get; set; }
        public int seqNum { get; set; }
        public string? SKU { get; set; }
        public int QtyTo { get; set; }
        public string? TrackIdTo { get; set; }
        public string? LocationTo { get; set; }
        public string? LpnTo { get; set; }
        public string? ProductCondition { get; set; }
        public string? ReceivingStatus { get; set; }
        public string? UomDisplay { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
