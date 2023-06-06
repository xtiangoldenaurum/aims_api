using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class LocationTypeModel
    {
		public string? LocationTypeId { get; set; }
		public string? LocationTypeName { get; set; }
		public string? Description { get; set; }
		public bool AllowPallet { get; set; }
		public bool SinglePallet { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
		public string? CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
	}
}
