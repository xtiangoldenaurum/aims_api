using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class AuditTrailModel
    {
		public int AuditId { get; set; }
		public DateTime AuditDate { get; set; }
		public string? UserAccountId { get; set; }
		public string? ActionTypeId { get; set; }
		public string? TransactionTypeId { get; set; }
		public string? RecordId { get; set; }
		public string? Data { get; set; }
		public string? Remarks { get; set; }
    }
}
