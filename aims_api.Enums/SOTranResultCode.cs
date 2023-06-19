using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum SOTranResultCode
    {
        [Description("SO transacted successfully.")]
        SUCCESS,
        [Description("Similar SKU already exists in current SO.")]
        SKUCONFLICT,
        [Description("Failed to save SO detail.")]
        SOLINESAVEFAILED,
        [Description("Failed to process current SO.")]
        FAILED,
        [Description("Primary reference number is in use.")]
        INVALIDREFNUMONE,
        [Description("Primary reference number is required.")]
        MISSINGREFNUMONE,
        [Description("Secondary reference number is in use.")]
        INVALIDREFNUMTWO,
        [Description("Failed to process SO user fields value.")]
        USRFIELDSAVEFAILED,
        [Description("Arrival Dates should be later than Order Date.")]
        INVALIDARRIVALDATE,
        [Description("Invalid Header, Use the Template.")]
        INVALIDHEADER,
        [Description("Order Date is required.")]
        ORDERDATEISREQUIRED
    }
}