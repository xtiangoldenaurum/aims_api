using aims_api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class PutawayWhTransHeadrLockResult
    {
        public PutawayResultCode ResultCode { get; set; } = PutawayResultCode.FAILED;
        public WhTransferModel? WhTransferHeader { get; set; }
    }
}
