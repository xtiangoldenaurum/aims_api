using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IOrganizationRepository
    {
        Task<IEnumerable<OrganizationModel>> GetOrganizationPg(int pageNum, int pageItem);
        Task<OrganizationPagedMdl?> GetOrgPaged(int pageNum, int pageItem);
        Task<OrganizationPagedMdl?> GetOrgSearchPaged(string searchKey, int pageNum, int pageItem);
        Task<Pagination?> GetOrgPageDetail(IDbConnection db, int pageNum, int pageItem, int rowCount);
        Task<Pagination?> GetOrgSearchPageDetail(IDbConnection db, string searchKey, int pageNum, int pageItem, int rowCount);
        Task<IEnumerable<OrganizationModel>> GetOrganizationPgSrch(string searchKey, int pageNum, int pageItem);
        Task<OrganizationModel> GetOrganizationById(string organizationId);
        Task<bool> OrganizationExists(string organizationId);
        Task<IEnumerable<OrganizationModel>> GetOrgPgByOrgTypId(string organizationTypeId, int pageNum, int pageItem);
        Task<IEnumerable<LocationModel>> GetOrgPgFiltered(OrganizationFilterMdl filter, int pageNum, int pageItem);
        Task<OrganizationPagedMdl?> GetOrgFilteredPaged(OrganizationFilterMdl filter, int pageNum, int pageItem);
        Task<bool> CreateOrganization(OrganizationModel organization);
        Task<bool> UpdateOrganization(OrganizationModel organization);
        Task<bool> OrganizationInUse(string organizationId);
        Task<bool> DeleteOrganization(string organizationId, string userAccountId);
    }
}
