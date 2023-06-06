using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum CancelWhTransResultCode
    {
        [Description("WH Transfer transacted successfully.")]
        SUCCESS,
        [Description("Failed to retreive or lock WH Transfer details.")]
        WHTRANSDETAILLOCKFAILED,
        [Description("Failed to rerteive or lock WH Transfer header.")]
        WHTRANSLOCKFAILED,
        [Description("WH Transfer details status/es modified.")]
        WHTRANSDETAILSSTATUSALTERED,
        [Description("WH Transfer header status is already modified.")]
        WHTRANSFERSTATUSALTERED,
        [Description("Failed to update WH Transfer header status.")]
        WHTRANSSTATUSUPDATEFAILED,
        [Description("Failed to update WH Transfer Detail/s status.")]
        WHTRANSDETAILSSTATUSUPDATEFAILED,
        [Description("WH Transfer has no force close-able details.")]
        WHTRANSDETAILSNOTVALID,
        [Description("WH Transfer has pending putaway task.")]
        HASPUTAWAYTASKPENDING,
        [Description("WH Transfer status should be in partial receive.")]
        WHTRANSSTATUSNOTVALID,
        [Description("Failed to cancel WH Transfer.")]
        FAILED
    }
}
