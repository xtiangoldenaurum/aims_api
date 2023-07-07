using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Enums
{
    public enum InvAdjustTranResultCode
    {
        [Description("Adjustment Document transacted successfully.")]
        SUCCESS,
        [Description("Similar SKU already exists in current Adjustment Document.")]
        SKUCONFLICT,
        [Description("Failed to save Adjustment Document detail.")]
        ADJUSTMENTDOCLINESAVEFAILED,
        [Description("Failed to process Inventory Adjustment user fields value.")]
        USRFIELDSAVEFAILED,
        [Description("Failed to process current Adjustment Document.")]
        FAILED,
        [Description("Adjustment Qty should be less than or equal to Actual Qty.")]
        INVALIDQTY, 
        [Description("Access rights are not applicable for non-admin users.")]
        INVALIDUSERACCESSRIGHT
    }
}
