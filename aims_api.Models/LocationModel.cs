using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class LocationModel
    {
        [Required(ErrorMessage = "Provide valid ID")]
        public string? LocationId { get; set; }
		public string? LocationName { get; set; }
		public string? Description { get; set; }
		public string? LocationTypeId { get; set; }
		public string? LocationGroupId { get; set; }
		public string? AreaId { get; set; }
		public string? ValidationCode { get; set; }
		public string? AisleCode { get; set; }
		public string? BayCode { get; set; }
		public int Inactive { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
		public string? CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
    }
}
