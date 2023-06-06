using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum PutawayResultCode
    {
        [Description("Putaway completed successfuly.")]
        SUCCESS,
        [Description("Failed to lock inventory history record.")]
        FAILEDTOLOCKINVHIST,
        [Description("Failed to retreive inventory history record.")]
        FAILEDTOGETINVHIST,
        [Description("Failed to lock inventory header.")]
        FAILEDTOLOCKINVHEAD,
        [Description("Inventory header status is not valid.")]
        INVALIDINVHEADERSTATUS,
        [Description("Failed to lock receive transaction.")]
        FAILEDTOLOCKRCVDETAILS,
        [Description("Failed to lock putaway task.")]
        FAILEDTOLOCKPTask,
        [Description("Current putaway task status is not valid.")]
        INVALIDPUTAWAYTASKSTATUS,
        [Description("Failed to define product description.")]
        FAILEDTOGETPRODDETAILS,
        [Description("Failed to define Lot Attributes.")]
        FAILEDTOGETLOTATT,
        [Description("Tracking ID details differed from first step process.")]
        RECORDINCONSISTENCY,
        [Description("Failed to lock linked PO detail.")]
        FAILEDTOLOCKPODETAIL,
        [Description("Failed to lock linked Returns detail.")]
        FAILEDTOLOCKRETDETAIL,
        [Description("Failed to lock linked WH Trnasfer detail.")]
        FAILEDTOLOCKWHTRANSDETAIL,
        [Description("Failed to lock linked PO header.")]
        FAILEDTOLOCKPOHEADER,
        [Description("Failed to lock linked Returns header.")]
        FAILEDTOLOCKRETHEADER,
        [Description("Failed to lock linked WH Transfer header.")]
        FAILEDTOLOCKWHTRANSHEADER,
        [Description("Putaway qty exceeeds available staging qty.")]
        QTYEXCEEDS,
        [Description("PO detail status is not valid.")]
        PODETAILSSTATUSNOTVALID,
        [Description("Returns detail status is not valid.")]
        RETDETAILSSTATUSNOTVALID,
        [Description("WH Transfer detail status is not valid.")]
        WHTRANSDETAILSSTATUSNOTVALID,
        [Description("PO header status is not valid.")]
        POHEADERSTATUSNOTVALID,
        [Description("Returns header status is not valid.")]
        RETHEADERSTATUSNOTVALID,
        [Description("WH Transfer header status is not valid.")]
        WHTRANSHEADERSTATUSNOTVALID,
        [Description("Failed to commit new sequence of inventory detail.")]
        INVHISTCOMMITFAILED,
        [Description("Failed to commit new reduced sequence of inventory detail.")]
        INVHISTREDUCEFAILED,
        [Description("Failed to update inventory header status.")]
        INVSTATUSUPDATEFAILED,
        [Description("Failed to close receiving status.")]
        RCVSTATUSUPDATEFAILED,
        [Description("Failed to close putaway task status.")]
        PUTAWAYSTATUSUPDATEFAILED,
        [Description("Failed to retreive update linked PO header status.")]
        FAILEDGETUPDATEPOSTATUS,
        [Description("Failed to retreive update linked Returns header status.")]
        FAILEDGETUPDATERETSTATUS,
        [Description("Failed to retreive update linked WH Transfer header status.")]
        FAILEDGETUPDATEWHTRANSSTATUS,
        [Description("Failed to update PO header status.")]
        FAILEDTOUPDATEPOSTATUS,
        [Description("Failed to update Returns header status.")]
        FAILEDTOUPDATERETSTATUS,
        [Description("Failed to update WH Transfer header status.")]
        FAILEDTOUPDATEWHTRANSSTATUS,
        [Description("Failed to update PO detail status.")]
        FAILEDTOUPDATEPODTLSTATUS,
        [Description("Failed to update Returns detail status.")]
        FAILEDTOUPDATERETDTLSTATUS,
        [Description("Failed to update WH Transfer detail status.")]
        FAILEDTOUPDATEWHTRANSDTLSTATUS,
        [Description("Failed to generate partial putaway inventory header.")]
        FAILEDTOCREATEINVHEADER,
        [Description("Failed to generate partial putaway inventory detail.")]
        FAILEDTOCREATEINVDETAIL,
        [Description("Failed to generate partial putaway receiving detail.")]
        FAILEDTOCREATERCVDETAIL,
        [Description("Failed to generate partial putaway task.")]
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
        [Description("Failed to perform putaway.")]
        FAILED,
        [Description("Cannot use receiving LPN Id as LPNTo (Try LPN Putaway instead).")]
        CANNOTUSESAMELPN,
        [Description("Cannot use a different LPN Id on Pallet putaway aside from its own LPN Id.")]
        CANNOTUSEOTHERLPN,
        [Description("LPN ID is already in used.")]
        LPNISALREADYUSED,
        [Description("Invalid LPN Id format.")]
        INVALIDLPNFORMAT,
        [Description("Select other putaway location.")]
        INVALIDLPNLOCAION,
        [Description("Faile to define document origin type.")]
        FAILEDTODEFINEDOCORIGINTYPE,
        [Description("Target location is same in the current location.")]
        TARGETLOCCONFLICT
    }
}
