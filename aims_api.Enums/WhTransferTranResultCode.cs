using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum WhTransferTranResultCode
    {
        [Description("WH Transfer transacted successfully.")]
        SUCCESS,
        [Description("Similar SKU already exists in current WH Transfer.")]
        SKUCONFLICT,
        [Description("Failed to save WH Transfer detail.")]
        WHTRANSLINESAVEFAILED,
        [Description("Failed to process current WH Transfer.")]
        FAILED,
        [Description("Primary reference number is in use.")]
        INVALIDREFNUMONE,
        [Description("Primary reference number is required.")]
        MISSINGREFNUMONE,
        [Description("Secondary reference number is in use.")]
        INVALIDREFNUMTWO,
        [Description("Failed to process WH Transfer user fields value.")]
        USRFIELDSAVEFAILED,
        [Description("Transfer Date should be later than Arrival Dates.")]
        INVALIDTRANSFERDATE,
        [Description("Invalid Header, Use the Template.")]
        INVALIDHEADER,
        [Description("Transfer Date is required.")]
        TRANSFERDATEISREQUIRED
    }
}