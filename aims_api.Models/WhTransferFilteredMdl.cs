using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class WhTransferFilteredMdl
    {
		public string? WhFromId { get; set; }
        public string? CarrierId { get; set; }
        public DateTime? TransferDate { get; set; }
        public string? WhTransStatusId { get; set; }
    }
}
