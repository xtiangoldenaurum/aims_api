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
        [Description("Secondary reference number is in use.")]
        INVALIDREFNUMTWO,
        [Description("Failed to process WH Transfer user fields value.")]
        USRFIELDSAVEFAILED
    }
}