using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class PutawayContainerModel
    {
        public string? Sku { get; set; }
        public string? ReceivingId { get; set; }
        public string? PutawayTaskId { get; set; }
        public string? UserAccountId { get; set; }
        public InventoryHistoryModel? InvHistory { get; set; }
        public PutawayTaskProcModel? WinData { get; set; }
        public LotAttributeDetailModel? LotAtt { get; set; }
        //public POModel? PO { get; set; }
        //public IEnumerable<PODetailModel>? PODetails { get; set; }
        //public ReturnsModel? Returns { get; set; }
        //public IEnumerable<ReturnsDetailModel>? ReturnsDetails { get; set; }
        //public WhTransferModel? WHTrans { get; set; }
        //public IEnumerable<WhTransferDetailModel>? WHTransDetails { get; set; }
    }
}
