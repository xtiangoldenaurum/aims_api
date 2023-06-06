using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Utilities
{
    public class AppSettings
    {
        public string? Key { get; set; }
        public int TokenExpiryByDay { get; set; }
        public string? TokenUsername { get; set; }
        public string? TokenPassword { get; set; }
    }
}
