using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Enums
{
    public enum ProceedMovementResultCode
    {
        //[Description("Movement Task transaction failed.")]
        //FAILED,
        //[Description("Movement Task completed sucessfully.")]
        //SUCCESS,
        //[Description("Invalid Qty")]
        //INVALIDQTY,
        //[Description("Invalid Location")]
        //INVALIDLOCATION,
        //[Description("Invalid LPN")]
        //INVALIDLPN,
        //[Description("Inventory Move status inconsistency.")]
        //INVALIDINVMOVE,
        //[Description("Inventory Move detail status inconsistency.")]
        //INVALIDINVMOVELANE,
        //[Description("There is no TID data to process.")]
        //INVALIDDATAGIVEN,

        [Description("Movement Task transaction failed.")]
        FAILED,
        [Description("Movement Task completed sucessfully.")]
        SUCCESS,
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
        [Description("There is no data to process.")]
        INVALIDDATAGIVEN
    }
}
