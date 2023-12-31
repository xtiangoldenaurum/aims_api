﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum ReturnsTranResultCode
    {
        [Description("Returns transacted successfully.")]
        SUCCESS,
        [Description("Similar SKU already exists in current Returns.")]
        SKUCONFLICT,
        [Description("Failed to save Returns detail.")]
        RETLINESAVEFAILED,
        [Description("Failed to process current Returns.")]
        FAILED,
        [Description("Primary reference number is in use.")]
        INVALIDREFNUMONE,
        [Description("Primary reference number is required.")]
        MISSINGREFNUMONE,
        [Description("Secondary reference number is in use.")]
        INVALIDREFNUMTWO,
        [Description("Failed to process Returns user fields value.")]
        USRFIELDSAVEFAILED,
        [Description("Return Date should be later than Arrival Dates.")]
        INVALIDRETURNDATE,
        [Description("Invalid Header, Use the Template.")]
        INVALIDHEADER,
        [Description("Return Date is required.")]
        RETURNDATEISREQUIRED
    }
}