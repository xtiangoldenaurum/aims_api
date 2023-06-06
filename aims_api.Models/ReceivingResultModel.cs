using aims_api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class ReceivingResultModel
    {
        public ReceivingResultCode ResultCode { get; set; }
        public IEnumerable<BCodeLabelToPrintModel>? LabelsToPrint { get; set; }
        public IEnumerable<ZPLDetail>? ZplDetails { get; set; }
    }
}
