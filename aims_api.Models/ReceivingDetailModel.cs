using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class ReceivingDetailModel
    {
        public string? ReceivingId { get; set; }
        public string? DocLineId { get; set; }
        public string? TrackIdTo { get; set; }
        public string? SKU { get; set; }
        public string? ProductName { get; set; }
        public int QtyTo { get; set; }
        public string? UOMDisplay { get; set; }
        public string? LocationTo { get; set; }
        public string? LPNTo { get; set; }
        public string? LotAttributeId { get; set; }
        public string? ReceivingStatusId { get; set; }
        public string? Remarks { get; set; }
    }
}
