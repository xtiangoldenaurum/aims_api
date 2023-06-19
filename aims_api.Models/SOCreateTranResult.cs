using aims_api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class SOCreateTranResult
    {
        public SOTranResultCode ResultCode { get; set; }
        public string? SOId { get; set; }
        public string[]? SOIds { get; set; }
    }
}
