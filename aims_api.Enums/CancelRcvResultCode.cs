using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum CancelRcvResultCode
    {
        [Description("Receive cancel failed.")]
        FAILED,
        [Description("Received transaction canceled successfully.")]
        SUCCESS,
        [Description("Failed to lock current received transaction.")]
        RCVLOCKFAIL,
        [Description("Failed to lock linked putaway task.")]
        PUTAWAYLOCKFAIL,
        [Description("Failed to lock root PO detail.")]
        PODETAILLOCKFAIL,
        [Description("Failed to lock root Returns detail.")]
        RETDETAILLOCKFAIL,
        [Description("Failed to lock root WH Transfer detail.")]
        WHTRANSDETAILLOCKFAIL,
        [Description("Failed to lock main PO.")]
        POLOCKFAIL,
        [Description("Failed to lock main Returns.")]
        RETLOCKFAIL,
        [Description("Failed to lock main WH Transfer.")]
        WHTRANSLOCKFAIL,
        [Description("Record is locked by other transaction.")]
        HASRECORDLOCKED,
        [Description("There's an inconsistency with current receive transaction.")]
        RCVINCONSISTENCY,
        [Description("There's an inconsistency with linked putaway task.")]
        PUTAWAYINCONSISTENCY,
        [Description("There's an inconsistency with root PO detail.")]
        PODTLINCONSISTENCY,
        [Description("There's an inconsistency with root Returns detail.")]
        RETDTLINCONSISTENCY,
        [Description("There's an inconsistency with root WH Transfer detail.")]
        WHTRANSDTLINCONSISTENCY,
        [Description("There's an inconsistenacy with main PO.")]
        POINCONSISTENCY,
        [Description("There's an inconsistenacy with main Returns.")]
        RETINCONSISTENCY,
        [Description("There's an inconsistenacy with main WH Transfer.")]
        WHTRANSINCONSISTENCY
    }
}
