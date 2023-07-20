using aims_api.Models.Heredities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class PODetailModelMod : PODetailModelHeredity
    {
		public string? PoLineId { get; set; }
		public string? PoId { get; set; }
		public string? Sku { get; set; }
		public int orderQty { get; set; }
		public string? PoLineStatusId { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
		public string? CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
		public string? Remarks { get; set; }
		public int TotalReceived { get; set; }
		public int QtyToReceived { get; set; }
	}
}
