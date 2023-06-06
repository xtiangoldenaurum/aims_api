using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class InventoryModel
    {
		public string? InventoryId { get; set; }
		public string? Sku { get; set; }
		public string? InventoryStatusId { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
    }
}
