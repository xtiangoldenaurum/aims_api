using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Enums
{
    public enum MovementResultCode
    {
        [Description("Movement completed successfuly.")]
        SUCCESS,
        [Description("Invalid Quantity")]
        INVALIDQTY,
        [Description("Invalid Location")]
        INVALIDLOC,
        [Description("Inventory Move status inconsistency.")]
        INVALIDINVMOVE,
        [Description("Inventory Move detail status inconsistency.")]
        INVALIDINVMOVELANE,
        [Description("Failed to lock inventory history record.")]
        FAILEDTOLOCKINVHIST,
        [Description("Failed to retreive inventory history record.")]
        FAILEDTOGETINVHIST,
        [Description("Failed to lock inventory header.")]
        FAILEDTOLOCKINVHEAD,
        [Description("Inventory header status is not valid.")]
        INVALIDINVHEADERSTATUS,
        [Description("Failed to lock movement transaction.")]
        FAILEDTOLOCKMVDETAILS,
        [Description("Failed to lock movement task.")]
        FAILEDTOLOCKMTask,
        [Description("Current movement task status is not valid.")]
        INVALIDMOVEMENTTASKSTATUS,
        [Description("Failed to define product description.")]
        FAILEDTOGETPRODDETAILS,
        [Description("Failed to define Lot Attributes.")]
        FAILEDTOGETLOTATT,
        [Description("Tracking ID details differed from first step process.")]
        RECORDINCONSISTENCY,
        [Description("Failed to lock linked Inventory Move detail.")]
        FAILEDTOLOCKINVMOVEDETAIL,
        [Description("Failed to lock linked Inventory Move header.")]
        FAILEDTOLOCKINVMOVEHEADER,
        [Description("Movement qty exceeeds.")]
        QTYEXCEEDS,
        [Description("Inventory Move detail status is not valid.")]
        INVMOVEDETAILSSTATUSNOTVALID,
        [Description("Inventory Move header status is not valid.")]
        INVMOVEHEADERSTATUSNOTVALID,
        [Description("Failed to commit new sequence of inventory detail.")]
        INVHISTCOMMITFAILED,
        [Description("Failed to commit new reduced sequence of inventory detail.")]
        INVHISTREDUCEFAILED,
        [Description("Failed to update inventory header status.")]
        INVSTATUSUPDATEFAILED,
        [Description("Failed to close movement status.")]
        MVSTATUSUPDATEFAILED,
        [Description("Failed to close movement task status.")]
        PUTAWAYSTATUSUPDATEFAILED,
        [Description("Failed to retreive update linked Inventory Move header status.")]
        FAILEDGETUPDATEINVMOVESTATUS,
        [Description("Failed to update Inventory Move header status.")]
        FAILEDTOUPDATEINVMOVESTATUS,
        [Description("Failed to update Inventory Move detail status.")]
        FAILEDTOUPDATEINVMOVEDTLSTATUS,
        [Description("Failed to generate partial movement inventory header.")]
        FAILEDTOCREATEINVHEADER,
        [Description("Failed to generate partial movement inventory detail.")]
        FAILEDTOCREATEINVDETAIL,
        [Description("Failed to generate partial movement detail.")]
        FAILEDTOCREATEMVDETAIL,
        [Description("Failed to generate partial movement task.")]
        FAILEDTOCREATEPUTAWAY,
        [Description("Invalid location validation code.")]
        INVALIDLOCVCODE,
        [Description("Invalid location code.")]
        INVALIDLOCCODE,
        [Description("Failed to retreive location type.")]
        FAILEDTOGETLOCTYPE,
        [Description("Pallet found on non-pallet allowed location.")]
        PALLETFOUNDONNONPALLETLOC,
        [Description("Multiple pallet found on single pallet restricted location.")]
        MULTIPALLETONSINGLEPALLETLOC,
        [Description("Failed to retreived LPNTo.")]
        FAILEDTOGETLPNTO,
        [Description("Putaway LPNTo differed from target location's insisted LPNTo.")]
        INVALIDLPNTO,
        [Description("Failed to perform movement.")]
        FAILED,
        [Description("Cannot use a different LPN Id on Pallet movement aside from its own LPN Id.")]
        CANNOTUSEOTHERLPN,
        [Description("LPN ID is already in used.")]
        LPNISALREADYUSED,
        [Description("Invalid LPN Id format.")]
        INVALIDLPNFORMAT,
        [Description("Select other movement location.")]
        INVALIDLPNLOCAION,
        [Description("Faile to define document origin type.")]
        FAILEDTODEFINEDOCORIGINTYPE,
        [Description("Target location is same in the current location.")]
        TARGETLOCCONFLICT
    }
}
