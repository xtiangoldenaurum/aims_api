using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IUtilitiesRepository
    {
        Task<string?> DefineTranTypeByDocId(string documentId);
        Task<string?> DefineTranTypeByDocLineId(string docLineId);
        Task<Tenant?> GetTenantSettings();
    }
}
