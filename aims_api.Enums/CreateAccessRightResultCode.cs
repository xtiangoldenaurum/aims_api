using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Enums
{
    public enum CreateAccessRightResultCode
    {
        [Description("Failed to create access rights header.")]
        FAILED,
        [Description("Access rights header created successfully.")]
        SUCCESS,
        [Description("Access rights Id alaready exsists.")]
        ACCESSRIGHTEXISTS,
        [Description("Access rights details create failed.")]
        FAILEDTOSAVEDETAILS
    }
}
