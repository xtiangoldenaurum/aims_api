using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class SODetailModel
    {
        public string? SoLineId { get; set; }
        public string? SoId { get; set; }
        public string? Sku { get; set; }
        public int orderQty { get; set; }
        public string? SoLineStatusId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? Remarks { get; set; }
    }
}
