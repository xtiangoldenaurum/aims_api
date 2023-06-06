using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class AccessRightHDModel //accessright header and details model
    {
        public AccessRightModel? Header { get; set; }
        public IEnumerable<AccessRightDetailModelMod>? Details { get; set; }
    }
}
