using aims_api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class AccValidationResult
    {
        public LoginResultCode ResultCode { get; set; } = LoginResultCode.FAILED;
        public UserAccountModel? UserAccount { get; set; }
        public IEnumerable<UserAllowedActionsModel>? MobUserAccessRights { get; set; }
        public IEnumerable<GroupedUserAllowedActions>? WebUserAccessRights { get; set; }
    }
}
