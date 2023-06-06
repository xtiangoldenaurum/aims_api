using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class AccessRightPagedMdl
    {
        public Pagination? Pagination { get; set; }
        public IEnumerable<AccessRightModel>? AccessRight { get; set; }
    }
}
