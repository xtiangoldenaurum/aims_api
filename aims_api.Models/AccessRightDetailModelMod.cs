using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class AccessRightDetailModelMod
    {
        public string? ModuleName { get; set; }
        public bool Enabled { get; set; }
        public string? HeaderModuleId { get; set; }
        public string? HeaderActionTypeId { get; set; }
        public int ActionSeqNum { get; set; }
        public bool Allow { get; set; }
        public string? AccessRightId { get; set; }
        public string? ModuleId { get; set; }
        public string? ActionTypeId { get; set; }
        public string? ActionName { get; set; }
        public string? EnvTypeId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
