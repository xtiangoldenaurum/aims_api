using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IOrganizationTypeRepository
    {
        Task<IEnumerable<OrganizationTypeModel>> GetOrganizationTypePg(int pageNum, int pageItem);
        Task<IEnumerable<OrganizationTypeModel>> GetOrganizationTypePgSrch(string searchKey, int pageNum, int pageItem);
        Task<OrganizationTypeModel> GetOrganizationTypeById(string organizationTypeID);
        Task<bool> OrganizationTypeExists(string organizationTypeID);
        Task<bool> CreateOrganizationType(OrganizationTypeModel organizationType);
        Task<bool> UpdateOrganizationType(OrganizationTypeModel organizationType);
        Task<bool> DeleteOrganizationType(string organizationTypeID);
    }
}
