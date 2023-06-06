using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class ProductConditionModel
    {
        [Required(ErrorMessage = "Provide valid ID")]
        public string? ProductConditionId { get; set; }
		public string? ProductCondition { get; set; }
		public string? Description { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
		public string? CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
    }
}
