using aims_api.Models.Heredities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class POModel: POModelHeredity
    {
		public string? PoId { get; set; }
		public string? RefNumber { get; set; }
		public string? RefNumber2 { get; set; }
		public string? SupplierId { get; set; }
		public string? SupplierName { get; set; }
		public string? SupplierAddress { get; set; }
		public string? SupplierContact { get; set; }
		public string? SupplierEmail { get; set; }
		public string? CarrierId { get; set; }
		public string? CarrierName { get; set; }
		public string? CarrierAddress { get; set; }
		public string? CarrierContact { get; set; }
		public string? CarrierEmail { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime? ArrivalDate { get; set; }
		public DateTime? ArrivalDate2 { get; set; }
		public string? PoStatusId { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime? DateModified { get; set; }
		public string? CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
		public string? Remarks { get; set; }
    }
}
