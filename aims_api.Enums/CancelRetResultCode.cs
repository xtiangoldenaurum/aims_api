using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum CancelRetResultCode
    {
        [Description("Returns transacted successfully.")]
        SUCCESS,
        [Description("Failed to retreive or lock Returns details.")]
        RETDETAILLOCKFAILED,
        [Description("Failed to rerteive or lock Returns header.")]
        RETLOCKFAILED,
        [Description("Returns details status/es modified.")]
        RETDETAILSSTATUSALTERED,
        [Description("Returns header status is already modified.")]
        RETSTATUSALTERED,
        [Description("Failed to update Returns header status.")]
        RETSTATUSUPDATEFAILED,
        [Description("Failed to update Returns Detail/s status.")]
        RETDETAILSSTATUSUPDATEFAILED,
        [Description("Returns has no force close-able details.")]
        RETDETAILSNOTVALID,
        [Description("returns has pending putaway task.")]
        HASPUTAWAYTASKPENDING,
        [Description("Returns status should be in partial receive.")]
        RETSTATUSNOTVALID,
        [Description("Failed to cancel Returns.")]
        FAILED
    }
}
