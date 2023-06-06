using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class SOModelMod
    {
        public SOModel? SOHeader { get; set; }
        public IEnumerable<SODetailModel>? SODetails { get; set; }
        public dynamic? SOUfields { get; set; }
    }
}
