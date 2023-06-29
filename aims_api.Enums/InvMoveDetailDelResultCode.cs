using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Enums
{
    public enum InvMoveDetailDelResultCode
    {
        [Description("Failed to delete record.")]
        FAILED,
        [Description("Record deleted successfully.")]
        SUCCESS,
        [Description("Inventory Movement detail is used or lock by other transaction.")]
        INVMOVEDTAILINUSED,
        [Description("Inventory Movement Detail status is modified.")]
        DETAILSTATUSMODIFIED
    }
}
