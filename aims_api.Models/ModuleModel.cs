using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class ModuleModel
    {
		public string? ModuleId { get; set; }
		public string? ModuleName { get; set; }
		public int SeqNum { get; set; }
		public string? Description { get; set; }
		public string? ParentId { get; set; }
		public string? Icon { get; set; }
		public byte[]? Image { get; set; }
		public string? Url { get; set; }
		public string? EnvTypeId { get; set; }
		public int IsParent { get; set; }
	}
}
