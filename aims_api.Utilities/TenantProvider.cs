using aims_api.Utilities.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Utilities
{
    public class TenantProvider : ITenantProvider
    {
        private readonly ITenantSource _tenantSource;
        private readonly string _host;

        public TenantProvider(ITenantSource tenantSource, IHttpContextAccessor accessor)
        {
            _tenantSource = tenantSource;
            _host = accessor.HttpContext.Request.Host.ToString().Split('.')[0];
        }

        public Tenant GetTenant()
        {
            var tenants = _tenantSource.ListTenants();

            if (tenants != null)
            {
                var t = tenants
                            .Where(t => t.Subdomain.ToLower() == _host.ToLower())
                            .FirstOrDefault();

                if (t != null)
                {
                    return t;
                }
            }

            //throw an error incase tenant domain not found
            throw new Exception($"Unrecognized Tenant. Contact Administrator for help. Tenant Name: [{_host}]");
        }

        public Tenant GetTenant(string tenantName)
        {
            var tenants = _tenantSource.ListTenants();

            if (tenants != null)
            {
                var t = tenants
                            .Where(t => t.Subdomain.ToLower() == tenantName.ToLower())
                            .FirstOrDefault();

                if (t != null)
                {
                    return t;
                }
            }

            //throw an error incase tenant domain not found
            throw new Exception($"Unrecognized Tenant. Contact Administrator for help. Tenant Name: [{_host}]");
        }
    }
}
