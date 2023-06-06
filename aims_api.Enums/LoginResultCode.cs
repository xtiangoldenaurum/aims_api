using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace aims_api.Enums
{
    public enum LoginResultCode
    {
        [Description("Login failed.")]
        FAILED,
        [Description("Login success.")]
        SUCCESS,
        [Description("Invalid user account or password.")]
        INVALIDUSER,
        [Description("Your account is inactive.")]
        INACTIVEACCOUNT,
        [Description("Your account is expired.")]
        EXPIREDACCOUNT,
        [Description("Failed to retreive user account deails")]
        FAILEDTOGETUSERDETAILS,
        [Description("Failed to retreive user access rights.")]
        FAILEDTOGETACCESSRIGHTS
    }
}