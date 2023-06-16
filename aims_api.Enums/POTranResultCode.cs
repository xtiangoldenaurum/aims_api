using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum POTranResultCode
    {
        [Description("PO transacted successfully.")]
        SUCCESS,
        [Description("Similar SKU already exists in current PO.")]
        SKUCONFLICT,
        [Description("Failed to save PO detail.")]
        POLINESAVEFAILED,
        [Description("Failed to process current PO.")]
        FAILED,
        [Description("Primary reference number is in use.")]
        INVALIDREFNUMONE,
        [Description("Primary reference number is required.")]
        MISSINGREFNUMONE,
        [Description("Secondary reference number is in use.")]
        INVALIDREFNUMTWO,
        [Description("Failed to process PO user fields value.")]
        USRFIELDSAVEFAILED,
        [Description("Arrival Dates should be later than Order Date.")]
        INVALIDARRIVALDATE,
        [Description("Invalid Header, Use the Template.")]
        INVALIDHEADER,
        [Description("Order Date is required.")]
        ORDERDATEISREQUIRED
    }
}