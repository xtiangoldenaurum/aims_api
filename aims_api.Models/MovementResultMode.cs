using aims_api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Models
{
    public class MovementResultMode
    {
        public MovementResultCode ResultCode { get; set; }
        public MovementTaskProcModel? Data { get; set; }
    }
}
