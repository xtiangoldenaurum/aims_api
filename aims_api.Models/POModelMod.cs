using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class POModelMod
    {
        public POModel? POHeader { get; set; }
        public IEnumerable<PODetailModel>? PODetails { get; set; }
        public dynamic? POUfields { get; set; }
    }
}
