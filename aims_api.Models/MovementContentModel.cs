using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class MovementContentModel
    {
        public string? InventoryId { get; set; }
        public int SeqNum { get; set; }
        public int QtyTo { get; set; }
        public string? TrackIdTo { get; set; }
        public string? LocationTo { get; set; }
        public string? LotAttributeId { get; set; }
        public string? LpnTo { get; set; }
        public string? InvMoveId { get; set; }
        public string? InvMoveStatusId { get; set; }
        public string? InventoryStatusId { get; set; }
        public string? Sku { get; set; }
        public string? ProductName { get; set; }
        public string? UomRef { get; set; }
        public string? UomDisplay { get; set; }
        public string? MovementTaskId { get; set; }
        public string? MovementStatusId { get; set; }
    }
}
