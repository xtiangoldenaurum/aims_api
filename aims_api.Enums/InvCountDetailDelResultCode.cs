using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Enums
{
    public enum InvCountDetailDelResultCode
    {
        // TO REVIEW ALL OF THIS ENUM
        [Description("Failed to delete record.")]
        FAILED,
        [Description("Record deleted successfully.")]
        SUCCESS,
        [Description("Inventory Count detail is used or lock by other transaction.")]
        INVCOUNTTAILINUSED,
        [Description("Inventory Count Detail status is modified.")]
        DETAILSTATUSMODIFIED
    }
}
