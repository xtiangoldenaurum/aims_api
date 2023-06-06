using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class PutawayTaskModel
    {
		public string? PutawayTaskId { get; set; }
		public string? ReceivingId { get; set; }
		public string? InventoryId { get; set; }
		public int SeqNum { get; set; }
        public string? PutawayStatusId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
