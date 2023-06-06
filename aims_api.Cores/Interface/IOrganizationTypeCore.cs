using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IOrganizationTypeCore
    {
        Task<RequestResponse> GetOrganizationTypePg(int pageNum, int pageItem);
        Task<RequestResponse> GetOrganizationTypePgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetOrganizationTypeById(string organizationTypeID);
        Task<RequestResponse> CreateOrganizationType(OrganizationTypeModel organizationType);
        Task<RequestResponse> UpdateOrganizationType(OrganizationTypeModel organizationType);
        Task<RequestResponse> DeleteOrganizationType(string organizationTypeID);
    }
}
