using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class ActionTypeModel
    {
		public string? ActionTypeId { get; set; }
		public string? ActionName { get; set; }
		public string? Description { get; set; }
        public int SeqNum { get; set; }
    }
}
