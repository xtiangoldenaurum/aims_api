using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum PalletPutawayResultCode
    {
        [Description("Process completed successfully.")]
        SUCCESS,
        [Description("Failed to query target pallet contents.")]
        FAILED,
        [Description("Target pallet does not contain any item.")]
        EMPTYPALLET,
        [Description("Inconsistency on pallet contents receiving status.")]
        CONFLICTEDRCVSTATUS,
        [Description("Inconsistency on pallet contents inventory status.")]
        CONFLICTEDINVSTATUS,
        [Description("Inconsistency on pallet contents putaway task status.")]
        CONFLICTEDPUTAWAYSTATUS,
        [Description("Problem on TID: ")]
        SPECIFICTIDISSUE,
        [Description("There is no TID data to process.")]
        INVALIDDATAGIVEN
    }
}