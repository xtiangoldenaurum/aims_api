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
        [Description("Similar Inventory ID already exists in current Movement Document.")]
        INVENTORYIDCONFLICT,
        [Description("Failed to save Movement Document detail.")]
        MOVEMENTDOCLINESAVEFAILED,
        [Description("Failed to process Inventory Movement user fields value.")]
        USRFIELDSAVEFAILED,
        [Description("Failed to process current Movement Document.")]
        FAILED,
        [Description("Movement Qty should be less than or equal to Actual Qty.")]
        INVALIDQTY
    }
}
