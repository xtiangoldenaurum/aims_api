using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class UserAccountFilterMdl
	{
		public string? AccessRightId { get; set; }
		public int? Inactive { get; set; }
		public DateTime? AccountExpiry { get; set; }
    }
}
