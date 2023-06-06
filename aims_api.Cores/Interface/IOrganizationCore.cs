using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IOrganizationCore
    {
        Task<RequestResponse> GetOrgSpecial(OrganizationFilterMdl filter, string? searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetOrganizationPg(int pageNum, int pageItem);
        Task<RequestResponse> GetOrganizationPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetOrgPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetOrgSearchPaged(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetOrganizationById(string organizationId);
        Task<RequestResponse> GetOrgPgByOrgTypId(string organizationTypeId, int pageNum, int pageItem);
        Task<RequestResponse> GetOrgPgFiltered(OrganizationFilterMdl filter, int pageNum, int pageItem);
        Task<RequestResponse> GetOrgFilteredPaged(OrganizationFilterMdl filter, int pageNum, int pageItem);
        Task<RequestResponse> CreateOrganization(OrganizationModel organization);
        Task<RequestResponse> UpdateOrganization(OrganizationModel organization);
        Task<RequestResponse> DeleteOrganization(string organizationId, string userAccountId);
    }
}
