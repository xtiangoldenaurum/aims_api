using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IEmailSvcRepository
    {
        Task<bool> SendEmail(Tenant tenant, string recipients, string subject, string emailBody);
    }
}
