using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class InvCountModelMod
    {
        public InvCountModel? InvCountHeader { get; set; }
        public IEnumerable<InvCountDetailModel>? InvCountDetails { get; set; }
        public dynamic? InvCountUfields { get; set; }
    }
}
