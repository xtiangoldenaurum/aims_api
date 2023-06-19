using aims_api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class ReturnsCreateTranResult
    {
        public ReturnsTranResultCode ResultCode { get; set; }
        public string? ReturnsId { get; set; }
        public string[]? ReturnsIds { get; set; }
    }
}
