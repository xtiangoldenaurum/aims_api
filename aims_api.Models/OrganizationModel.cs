using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class OrganizationModel
    {
        [Required(ErrorMessage = "Provide valid ID")]
        public string? OrganizationId { get; set; }
		public string? OrganizationName { get; set; }
		public string? OrganizationTypeID { get; set; }
		public string? Address { get; set; }
		public string? Address2 { get; set; }
		public string? Telephone { get; set; }
		public string? Phone { get; set; }
		public string? Email { get; set; }
		public string? Province { get; set; }
		public string? City { get; set; }
		public string? ZipCode { get; set; }
		public int Inactive { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
		public string? CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
		public string? Remarks { get; set; }
    }
}
