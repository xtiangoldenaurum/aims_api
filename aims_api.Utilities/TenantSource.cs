using aims_api.Utilities.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Utilities
{
    public class TenantSource : ITenantSource
    {
        public Tenant[]? ListTenants()
        {
            var tenants = File.ReadAllText("tenants.json");
            return JsonConvert.DeserializeObject<Tenant[]>(tenants);
        }
    }
}
