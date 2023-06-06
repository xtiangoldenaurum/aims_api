using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class WarehouseInfoModel
    {
		public string? WarehouseId { get; set; }
		public string? WarehouseName { get; set; }
		public string? Description { get; set; }
		public string? Address { get; set; }
		public string? Address2 { get; set; }
		public string? Province { get; set; }
		public string? City { get; set; }
		public string? ZipCode { get; set; }
		public string? Telephone { get; set; }
		public string? Phone { get; set; }
		public string? Email { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
		public string? CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
    }
}
