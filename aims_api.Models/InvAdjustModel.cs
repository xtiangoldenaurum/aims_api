using aims_api.Models.Heredities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class InvAdjustModel : InvAdjustModelHeredity
    {
        public string? InvAdjustId { get; set; }
        public string? InvAdjustStatusId { get; set; }
        public string? WarehouseId { get; set; }
        public string? ReasonCodeId { get; set; }
        public string? Reason { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? Remarks { get; set; }
    }
}
