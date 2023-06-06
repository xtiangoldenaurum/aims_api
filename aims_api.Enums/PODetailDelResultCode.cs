using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum PODetailDelResultCode
    {
        [Description("Failed to delete record.")]
        FAILED,
        [Description("Record deleted successfully.")]
        SUCCESS,
        [Description("PO detail is used or lock by other transaction.")]
        PODTAILINUSED,
        [Description("PO Detail status is modified.")]
        DETAILSTATUSMODIFIED
    }
}