using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class SOModel
    {
		public string? SoId { get; set; }
		public string? SoTypeId { get; set; }
		public string? RefNumber { get; set; }
		public string? RefNumber2 { get; set; }
		public string? ConsigneeId { get; set; }
		public string? ConsigneeName { get; set; }
		public string? ConsigneeAddress { get; set; }
		public string? ConsigneeContact { get; set; }
		public string? ConsigneeEmail { get; set; }
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
		public DateTime OrderCreateDate { get; set; }
		public DateTime? ArrivalDate { get; set; }
		public DateTime? ArrivalDate2 { get; set; }
		public string? SoGrossWeight { get; set; }
		public int? ItemTotalQty { get; set; }
		public string? SoStatusId { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime? DateModified { get; set; }
		public string? CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
		public string? Remarks { get; set; }
    }
}
