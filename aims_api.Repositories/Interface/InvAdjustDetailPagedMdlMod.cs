using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public class InvAdjustDetailPagedMdlMod
    {
        public Pagination? Pagination { get; set; }
        public IEnumerable<InvAdjustDetailModelMod>? InvAdjustDetailModel { get; set; }
    }
}
