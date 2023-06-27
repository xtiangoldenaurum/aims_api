using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class InvMoveModelMod
    {
        public InvMoveModel? InvMoveHeader { get; set; }
        public IEnumerable<InvMoveDetailModel>? InvMoveDetails { get; set; }
        public dynamic? InvMoveUfields { get; set; }
    }
}
