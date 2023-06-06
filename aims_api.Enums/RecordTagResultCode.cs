using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum RecordTagResultCode
    {
        [Description("Failed to record unique tags details.")]
        FAILED,
        [Description("Unique tags recorded successfuly.")]
        SUCCESS,
        [Description("Target Tag Id has similar existing record.")]
        TAGIDCONFLICT,
        [Description("Target EPC has similar existing record.")]
        EPCCONFLICT,
        [Description("Target SN has similar existing record.")]
        SERIALNUMBERCONFLICT,
        [Description("Failed to log Target SN audit.")]
        FAILEDTOLOGAUDIT,
        [Description("Invalid unique tag entry.")]
        INVALIDTAGENTRY,
        [Description("Unique tag has conflict not a recaptured type.")]
        CONFLICTEDRACAPTURE
    }
}