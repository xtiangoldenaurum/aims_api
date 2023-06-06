using aims_api.Models.Heredities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class ReturnsModel : ReturnsModelHeredity
    {
		public string? ReturnsId { get; set; }
		public string? RefNumber { get; set; }
		public string? RefNumber2 { get; set; }
		public string? StoreId { get; set; }
		public string? StoreFrom { get; set; }
		public string? StoreAddress { get; set; }
		public string? StoreContact { get; set; }
		public string? StoreEmail { get; set; }
        public string? CarrierId { get; set; }
        public string? CarrierName { get; set; }
        public string? CarrierAddress { get; set; }
        public string? CarrierContact { get; set; }
        public string? CarrierEmail { get; set; }
		public DateTime ReturnDate { get; set; }
		public DateTime? ArrivalDate { get; set; }
		public DateTime? ArrivalDate2 { get; set; }
		public string? ReturnsStatusId { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
		public string? CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
		public string? Remarks { get; set; }
    }
}
