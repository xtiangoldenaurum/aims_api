using aims_api.Models.Heredities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class InvAdjustDetailModel : InvAdjustDetailModelHeredity
    {
        public string? InvAdjustLineId { get; set; }
        public string? InvAdjustId { get; set; }
        public string? Sku { get; set; }
        public int QtyFrom { get; set; }
        public int QtyTo { get; set; }
        public string? LocationFrom { get; set; }
        public string? ScanLocation { get; set; }
        public string? TrackingIdFrom { get; set; }
        public string? ScanTrackingId { get; set; }
        public string? InvMoveLineStatusId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? Remarks { get; set; }
    }
}
