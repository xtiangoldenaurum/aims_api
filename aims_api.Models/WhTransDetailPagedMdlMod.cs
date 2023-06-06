using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class WhtransDetailPagedMdlMod
    {
        public Pagination? Pagination { get; set; }
        public IEnumerable<WhTransferDetailModelMod>? WhTransDetailModel { get; set; }
    }
}
