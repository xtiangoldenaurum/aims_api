using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class RunningBalanceModel
    {
		public string? MovementTypeId { get; set; }
		public string? DocumentRefId { get; set; }
		public string? Sku { get; set; }
		public int QtyCommited { get; set; }
		public DateTime DateCreated { get; set; }
    }
}
