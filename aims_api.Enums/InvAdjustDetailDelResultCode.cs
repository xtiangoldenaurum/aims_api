using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Enums
{
    public enum InvAdjustDetailDelResultCode
    {
        [Description("Failed to delete record.")]
        FAILED,
        [Description("Record deleted successfully.")]
        SUCCESS,
        [Description("Inventory Adjustment detail is used or lock by other transaction.")]
        INVADJUSTDTAILINUSED,
        [Description("Inventory Adjustment Detail status is modified.")]
        DETAILSTATUSMODIFIED
    }
}
