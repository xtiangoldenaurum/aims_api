using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public enum CancelMvResultCode
    {
        [Description("Movement Task cancel failed.")]
        FAILED,
        [Description("Movement Task transaction cancelled successfully.")]
        SUCCESS,
        [Description("Failed to lock current movement task transaction.")]
        MVLOCKFAIL,
        [Description("Failed to lock linked putaway task.")]
        PUTAWAYLOCKFAIL,
        [Description("Failed to lock root Inventory Movement detail.")]
        INVMOVEDETAILLOCKFAIL,
        [Description("Failed to lock main Inventory Movement.")]
        INVMOVELOCKFAIL,
        [Description("Record is locked by other transaction.")]
        HASRECORDLOCKED,
        [Description("There's an inconsistency with current movement transaction.")]
        MVINCONSISTENCY,
        [Description("There's an inconsistency with root Inventory Movement detail.")]
        INVMOVEDTLINCONSISTENCY,
        [Description("There's an inconsistenacy with main Inventory Movement.")]
        INVMOVEINCONSISTENCY
    }
}
