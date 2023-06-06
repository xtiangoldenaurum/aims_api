using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class ZPLDetail
    {
        public string? DocTypeId { get; set; }
        public string? ZPLName { get; set; }
        public string? DPISizeId { get; set; }
        public byte[]? ZPLCode { get; set; }
        public string? ZPLLines { get; set; }
        public string? EPC { get; set; }
        public string? UntouchedEPC { get; set; }
    }
}
