using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum CreateUserAccResultCode
    {
        [Description("Failed to create user account.")]
        FAILED,
        [Description("User account created successfully.")]
        SUCCESS,
        [Description("Similar User Account Id exists")]
        SIMILARUSERACCID,
        [Description("Invalid user account data")]
        INVALIDUSERACCDATA,
        [Description("Failed to process Returns user fields value.")]
        USRFIELDSAVEFAILED
    }
}