using aims_api.Enums;
using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public class MovementResultModel
    {
        public MovementResultCode ResultCode { get; set; }
        public MovementTaskProcModel? Data { get; set; }
    }
}
