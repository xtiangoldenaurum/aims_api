using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Utilities
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Subdomain { get; set; } = String.Empty;
        public string SqlConnectionString { get; set; } = String.Empty;
        public string? Name { get; set; }
        public string DBName { get; set; } = String.Empty;
        public int NotifByBranch { get; set; }
        public string? SenderEmail { get; set; }
        public string? SenderKey { get; set; }
        public string? SMTP { get; set; }
        public int Port { get; set; }
        public string TagEPCConversion { get; set; } = String.Empty;
        public string EPCViewConversion { get; set; } = String.Empty;
        public string EPCPrintConversion { get; set; } = String.Empty;
        public bool LogPostData { get; set; }
    }
}
