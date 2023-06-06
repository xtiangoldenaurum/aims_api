using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class ReturnsFilteredMdl
    {
		public string? StoreId { get; set; }
        public string? CarrierId { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? ReturnsStatusId { get; set; }
    }
}
