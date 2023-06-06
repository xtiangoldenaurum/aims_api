using aims_api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class QryPalletPutawayResult
    {
        public PalletPutawayResultCode ResultCode { get; set; } = PalletPutawayResultCode.FAILED;
        public IEnumerable<PalletContentModel>? Data { get; set; }
    }
}
