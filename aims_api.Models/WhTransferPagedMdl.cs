using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class WhTransferPagedMdl
    {
        public Pagination? Pagination { get; set; }
        public IEnumerable<WhTransferModel>? WhTransfer { get; set; }
    }
}
