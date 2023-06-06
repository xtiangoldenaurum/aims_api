using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum CancelPOResultCode
    {
        [Description("PO transacted successfully.")]
        SUCCESS,
        [Description("Failed to retreive or lock PO details.")]
        PODETAILLOCKFAILED,
        [Description("Failed to rerteive or lock PO header.")]
        POLOCKFAILED,
        [Description("PO details status/es modified.")]
        PODETAILSSTATUSALTERED,
        [Description("PO header status is already modified.")]
        POSTATUSALTERED,
        [Description("Failed to update PO header status.")]
        POSTATUSUPDATEFAILED,
        [Description("Failed to update PO Detail/s status.")]
        PODETAILSSTATUSUPDATEFAILED,
        [Description("PO has no force close-able details.")]
        PODETAILSNOTVALID,
        [Description("PO has pending putaway task.")]
        HASPUTAWAYTASKPENDING,
        [Description("PO status should be in partial receive.")]
        POSTATUSNOTVALID,
        [Description("Failed to cancel PO.")]
        FAILED
    }
}
