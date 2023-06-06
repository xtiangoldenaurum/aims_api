using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum TargetLocResultCode
    {
        [Description("Failed to check target location details.")]
        FAILED,
        [Description("Target location defined successfuly.")]
        SUCCESS,
        [Description("Invalid location validation code.")]
        INVALIDLOCVCODE,
        [Description("Failed to retreive location type.")]
        FAILEDTOGETLOCTYPE,
        [Description("Pallet found on non-pallet allowed location.")]
        PALLETFOUNDONNONPALLETLOC,
        [Description("Multiple pallet found on single pallet restricted location.")]
        MULTIPALLETONSINGLEPALLETLOC,
        [Description("Failed to retreived LPNTo.")]
        FAILEDTOGETLPNTO,
        [Description("PO Detail status is modified.")]
        DETAILSTATUSMODIFIED
    }
}