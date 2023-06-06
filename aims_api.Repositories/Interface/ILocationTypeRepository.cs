using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface ILocationTypeRepository
    {
        Task<IEnumerable<LocationTypeModel>> GetLocationTypePg(int pageNum, int pageItem);
        Task<IEnumerable<LocationTypeModel>> GetLocationTypePgSrch(string searchKey, int pageNum, int pageItem);
        Task<LocationTypeModel> GetLocationTypeById(string locationTypeId);
        Task<bool> LocationTypeExists(string locationTypeId);
        Task<bool> CreateLocationType(LocationTypeModel locationType);
        Task<bool> UpdateLocationType(LocationTypeModel locationType);
        Task<bool> DeleteLocationType(string locationTypeId, string userAccountId);
    }
}
