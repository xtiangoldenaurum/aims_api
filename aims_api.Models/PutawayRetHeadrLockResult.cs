using aims_api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class PutawayRetHeadrLockResult
    {
        public PutawayResultCode ResultCode { get; set; } = PutawayResultCode.FAILED;
        public ReturnsModel? ReturnsHeader { get; set; }
    }
}
