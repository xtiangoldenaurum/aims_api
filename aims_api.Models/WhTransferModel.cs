using aims_api.Models.Heredities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class WhTransferModel : WhTransferModelHeredity
    {
		public string? WhTransferId { get; set; }
		public string? RefNumber { get; set; }
		public string? RefNumber2 { get; set; }
        public string? WhFromId { get; set; }
		public string? WhFrom { get; set; }
        public string? WhFromAddress { get; set; }
		public string? WhFromContact { get; set; }
		public string? WhFromEmail { get; set; }
        public string? CarrierId { get; set; }
        public string? CarrierName { get; set; }
        public string? CarrierAddress { get; set; }
        public string? CarrierContact { get; set; }
        public string? CarrierEmail { get; set; }
        public DateTime TransferDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
		public DateTime? ArrivalDate2 { get; set; }
		public string? WhTransStatusId { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
		public string? CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
		public string? Remarks { get; set; }
    }
}
