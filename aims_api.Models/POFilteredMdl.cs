using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class POFilteredMdl
    {
		public string? SupplierId { get; set; }
        public string? CarrierId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? PoStatusId { get; set; }
    }
}
