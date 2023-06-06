using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class WhTransferModelMod
    {
        public WhTransferModel? whTransferHeader { get; set; }
        public IEnumerable<WhTransferDetailModel>? WhTransDetails { get; set; }
        public dynamic? WhTransferUfields { get; set; }
    }
}
