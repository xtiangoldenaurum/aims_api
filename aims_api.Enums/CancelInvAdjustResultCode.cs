using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Enums
{
    public enum CancelInvAdjustResultCode
    {
        [Description("Adjustment Document transacted successfully.")]
        SUCCESS,
        [Description("Failed to retreive or lock Adjustment Document details.")]
        INVADJUSTDETAILLOCKFAILED,
        [Description("Failed to rerteive or lock Adjustment Document header.")]
        INVADJUSTLOCKFAILED,
        [Description("Adjustment Document details status/es modified.")]
        INVADJUSTDETAILSSTATUSALTERED,
        [Description("Adjustment Document header status is already modified.")]
        INVADJUSTSTATUSALTERED,
        [Description("Failed to update Adjustment Document header status.")]
        INVADJUSTSTATUSUPDATEFAILED,
        [Description("Failed to update Adjustment Document Detail/s status.")]
        INVADJUSTDETAILSSTATUSUPDATEFAILED,
        [Description("Adjustment Document has no force close-able details.")]
        INVADJUSTDETAILSNOTVALID,
        [Description("Adjustment Document has pending adjustment.")]
        HASADJUSTMENTPENDING,
        [Description("Adjustment Document status should be in partial adjust.")]
        INVADJUSTSTATUSNOTVALID,
        [Description("Failed to cancel Adjustment Document.")]
        FAILED
    }
}
