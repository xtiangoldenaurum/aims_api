using aims_api.Models.Heredities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class InventoryHistoryModel : InvHistoryModelHeredity
    {
		public string? InventoryId { get; set; }
		public int SeqNum { get; set; }
		public string? DocumentRefId { get; set; }
		public int QtyFrom { get; set; }
		public int QtyTo { get; set; }
		public string? LocationFrom { get; set; }
		public string? LocationTo { get; set; }
		public string? TrackIdFrom { get; set; }
		public string? TrackIdTo { get; set; }
		public string? LpnFrom { get; set; }
		public string? LpnTo { get; set; }
		public string? LotAttributeId { get; set; }
		public string? TransactionTypeId { get; set; }
		public string? Remarks { get; set; }
		public DateTime DateCreated { get; set; }
		public string? CreatedBy { get; set; }
    }
}
