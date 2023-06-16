using aims_api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class POCreateTranResult
    {
        public POTranResultCode ResultCode { get; set; }
        public string? POId { get; set; }
        public string[] POIds { get; set; }
    }
}
