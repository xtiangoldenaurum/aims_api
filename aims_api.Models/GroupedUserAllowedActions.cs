using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class GroupedUserAllowedActions
    {
        public string? ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public string? Actions { get; set; }
        public int SeqNum { get; set; }
        public string? ParentId { get; set; }
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public byte[]? Image { get; set; }
        public string? EnvTypeId { get; set; }
        public List<UserAllowedActionsModel>? ChildModules { get; set; }
    }
}
