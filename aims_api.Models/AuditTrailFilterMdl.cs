using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class AuditTrailFilterMdl
    {
        public string? RecordId { get; set; }
        public string? UserAccountId { get; set; }
        public string? TransactionTypeId { get; set; }
        public DateTime? AuditDateFrom { get; set; }
        public DateTime? AuditDateTo { get; set; }
    }
}
