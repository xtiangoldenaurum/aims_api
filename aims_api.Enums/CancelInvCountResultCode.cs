using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Enums
{
    public enum CancelInvCountResultCode
    {
        // TO REVIEW ALL OF THIS ENUM
        [Description("Count Document transacted successfully.")]
        SUCCESS,
        [Description("Failed to retreive or lock Count Document details.")]
        INVCOUNTDETAILLOCKFAILED,
        [Description("Failed to rerteive or lock Count Document header.")]
        INVCOUNTLOCKFAILED,
        [Description("Count Document details status/es modified.")]
        INVCOUNTDETAILSSTATUSALTERED,
        [Description("Count Document header status is already modified.")]
        INVCOUNTSTATUSALTERED,
        [Description("Failed to update Count Document header status.")]
        INVCOUNTSTATUSUPDATEFAILED,
        [Description("Failed to update Count Document Detail/s status.")]
        INVCOUNTDETAILSSTATUSUPDATEFAILED,
        [Description("Count Document has no force close-able details.")]
        INVCOUNTDETAILSNOTVALID,
        [Description("Count Document has pending move task.")]
        HASCOUNTTASKPENDING,
        [Description("Count Document status should be in partial move.")]
        INVCOUNTSTATUSNOTVALID,
        [Description("Failed to cancel Count Document.")]
        FAILED
    }
}
