using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class POPagedMdl
    {
        public Pagination? Pagination { get; set; }
        public IEnumerable<POModel>? PO { get; set; }
    }
}
