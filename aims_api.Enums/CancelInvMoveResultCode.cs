using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Enums
{
    public enum CancelInvMoveResultCode
    {
        [Description("Movement Document transacted successfully.")]
        SUCCESS,
        [Description("Failed to retreive or lock Movement Document details.")]
        INVMOVEDETAILLOCKFAILED,
        [Description("Failed to rerteive or lock Movement Document header.")]
        INVMOVELOCKFAILED,
        [Description("Movement Document details status/es modified.")]
        INVMOVEDETAILSSTATUSALTERED,
        [Description("Movement Document header status is already modified.")]
        INVMOVESTATUSALTERED,
        [Description("Failed to update Movement Document header status.")]
        INVMOVESTATUSUPDATEFAILED,
        [Description("Failed to update Movement Document Detail/s status.")]
        INVMOVEDETAILSSTATUSUPDATEFAILED,
        [Description("Movement Document has no force close-able details.")]
        INVMOVEDETAILSNOTVALID,
        [Description("Movement Document has pending move task.")]
        HASMOVETASKPENDING,
        [Description("Movement Document status should be in partial move.")]
        INVMOVESTATUSNOTVALID,
        [Description("Failed to cancel Movement Document.")]
        FAILED
    }
}
