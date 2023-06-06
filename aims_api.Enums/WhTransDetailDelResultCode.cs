using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum WhTransDetailDelResultCode
    {
        [Description("Failed to delete record.")]
        FAILED,
        [Description("Record deleted successfully.")]
        SUCCESS,
        [Description("WH Transfer detail is used or lock by other transaction.")]
        WHTRANSDTAILINUSED,
        [Description("WH Transfer Detail status is modified.")]
        DETAILSTATUSMODIFIED
    }
}