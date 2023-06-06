using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class UserAccountModel
    {
        [Required(ErrorMessage = "Provide valid ID")]
        public string? UserAccountId { get; set; }
		public string? Password { get; set; }
		public string? AccessRightId { get; set; }
		public string? FirstName { get; set; }
		public string? MiddleInitial { get; set; }
		public string? LastName { get; set; }
		public byte[]? Image { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public int Inactive { get; set; }
		public DateTime? AccountExpiry { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
		public string? CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
    }
}
