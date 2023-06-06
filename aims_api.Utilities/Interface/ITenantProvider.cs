using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Utilities.Interface
{
    public interface ITenantProvider
    {
        Tenant GetTenant();
        Tenant GetTenant(string tenantName);
    }
}
