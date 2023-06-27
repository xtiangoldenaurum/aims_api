using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class InvAdjustModelMod
    {
        public InvAdjustModel? InvAdjustHeader { get; set; }
        public IEnumerable<InvAdjustDetailModel>? InvAdjustDetails { get; set; }
        public dynamic? InvAdjustUfields { get; set; }
    }
}
