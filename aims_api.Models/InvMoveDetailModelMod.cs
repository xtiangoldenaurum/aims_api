using aims_api.Models.Heredities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class InvMoveDetailModelMod : InvMoveDetailModelHeredity
    {
        public string? InvMoveLineId { get; set; }
        public string? InvMoveId { get; set; }
        public string? InvMoveLineStatusId { get; set; }
        public string? InventoryId { get; set; }
        public int? QtyTo { get; set; }
        public int? FinalQty { get; set; }
        public string? LocationTo { get; set; }
        public string? FinalLocation { get; set; }
        public string? LpnTo { get; set; }
        public string? FinalLpn { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? Remarks { get; set; }
    }
}
