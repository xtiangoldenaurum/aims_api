using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class UniqueTagsModel
    {
		public int UniqueTagId { get; set; }
		public string? trackId { get; set; }
		public string? TagId { get; set; }
		public string? EPC { get; set; }
		public string? Serialno { get; set; }
		public string? DocumentRefId { get; set; }
		public string? DocLineRefId { get; set; }
		public string? TransactionTypeId { get; set; }
		public DateTime DateCreated { get; set; }
		public string? CreatedBy { get; set; }
		public string? Remarks { get; set; }
    }
}
