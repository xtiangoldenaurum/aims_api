using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Enums
{
    public enum InvMoveTranResultCode
    {
        [Description("Movement Document transacted successfully.")]
        SUCCESS,
        [Description("Similar SKU already exists in current Movement Document.")]
        SKUCONFLICT,
        [Description("Failed to save Movement Document detail.")]
        MOVEMENTDOCLINESAVEFAILED,
        [Description("Failed to process PO user fields value.")]
        USRFIELDSAVEFAILED,
        [Description("Failed to process current Movement Document.")]
        FAILED
    }
}
