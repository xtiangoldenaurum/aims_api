using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Enums
{
    public enum MovementTaskResultCode
    {
        [Description("Movement Task transaction failed.")]
        FAILED,
        [Description("Movement Task completed sucessfully.")]
        SUCCESS,
        [Description("Invalid Qty")]
        INVALIDQTY,
        [Description("Invalid Location")]
        INVALIDLOCATION,
        [Description("Invalid LPN")]
        INVALIDLPN
    }
}
