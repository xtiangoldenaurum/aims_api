using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class LocationFilterMdl
	{
		public string? LocationTypeId { get; set; }
		public string? LocationGroupId { get; set; }
		public string? AreaId { get; set; }
        public int? Inactive { get; set; }
        public string? AisleCode { get; set; }
        public string? BayCode { get; set; }
    }
}
