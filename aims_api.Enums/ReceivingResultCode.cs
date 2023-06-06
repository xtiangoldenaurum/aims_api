using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum ReceivingResultCode
    {
        [Description("Received transaction failed.")]
        FAILED,
        [Description("Receving completed sucessfully.")]
        SUCCESS,
        [Description("Track Id is already in use.")]
        INVALIDTID,
        [Description("Pallet Id is already used outside inbound staging.")]
        INVALIDLPNID,
        [Description("Invalid quantity.")]
        INVALIDQTY,
        [Description("Record is held by other transaction.")]
        LOCKED,
        [Description("PO status inconsistency.")]
        INVALIDPO,
        [Description("PO detail status inconsistency.")]
        INVALIDPOLANE,
        [Description("Invalid LotAttribute date (Check expiry and manufacure date).")]
        LOTINVALIDDATE,
        [Description("Record inconsistency causing to fail current transaction.")]
        INCONSISTENCY,
        [Description("Invalid product condition.")]
        INVALIDCONDITION,
        [Description("Invalid LPN Id format.")]
        INVALIDLPNFORMAT,
        [Description("Failed to retreive document id prefix.")]
        FAILEDGETDOCIDPREFIX,
        [Description("Invalid Trace Id format.")]
        INVALIDTIDFORMAT,
        [Description("Failed to intiate Serial Numbers to print list.")]
        SNTOPRINTBUILDFAILED,
        [Description("Failed record captured unique IDs (check possible record conflict).")]
        UNIQUETAGSAVEFIALED,
        [Description("Unique IDs should be less or equal on item quantity to receive.")]
        UNIQTAGSEXCEEDSQTYTORCV,
        [Description("Failed to delete unique tag/s under this receive transaction.")]
        FAILEDTODELETEUNIQTAGS,
        [Description("Current location cannot be same on target location.")]
        TARGETLOCCONFLICT
    }
}
