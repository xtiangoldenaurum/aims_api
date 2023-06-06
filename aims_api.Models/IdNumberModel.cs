using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class IdNumberModel
    {
		public string? TransactionTypeId { get; set; }
		public string? Prefix { get; set; }
		public string? Suffix { get; set; }
		public int StartId { get; set; }
		public int LastId { get; set; }
		public string? ZeroesLength { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
		public string? CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
		public string? Remarks { get; set; }
    }
}
