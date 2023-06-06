using aims_api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Utilities
{
    public class RequestResponse
    {
        public ResponseCode Code { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }

        public RequestResponse(ResponseCode code, string message, object? data = null)
        {
            Code = code;
            Message = message;
            Data = data;
        }
    }
}
