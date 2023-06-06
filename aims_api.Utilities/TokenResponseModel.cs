using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Utilities
{
    public class TokenResponseModel
    {
        public string AuthToken { get; set; } = string.Empty;
        public DateTime TokenExpiry { get; set; }
    }
}
