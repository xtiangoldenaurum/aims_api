using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Enums
{
    public enum InvCountTranResultCode
    {
        // TO REVIEW ALL OF THIS ENUM
        [Description("Count Document transacted successfully.")]
        SUCCESS,
        [Description("Similar Inventory Id already exist and isn't closed.")]
        INVENTORYIDCONFLICT,
        [Description("Failed to save Count Document detail.")]
        COUNTDOCLINESAVEFAILED,
        [Description("Failed to process Inventory Count user fields value.")]
        USRFIELDSAVEFAILED,
        [Description("Failed to process current Count Document.")]
        FAILED,
        [Description("Target location is same in the current location.")]
        TARGETLOCCONFLICT
    }
}
