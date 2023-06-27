using aims_api.Models.Heredities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class InvMoveModel : InvMoveModelHeredity
    {
        public string? InvMoveId { get; set; }
        public string? InvMoveStatusId { get; set; }
        public string? WarehouseId { get; set; }
        public string? ReasonCodeId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? Remarks { get; set; }
    }
}
