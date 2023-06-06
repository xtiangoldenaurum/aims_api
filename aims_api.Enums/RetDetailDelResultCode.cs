using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum RetDetailDelResultCode
    {
        [Description("Failed to delete record.")]
        FAILED,
        [Description("Record deleted successfully.")]
        SUCCESS,
        [Description("Returns detail is used or lock by other transaction.")]
        RETDTAILINUSED,
        [Description("Returns Detail status is modified.")]
        DETAILSTATUSMODIFIED
    }
}