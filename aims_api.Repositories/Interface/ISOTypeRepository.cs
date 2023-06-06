using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface ISOTypeRepository
    {
        Task<IEnumerable<SOTypeModel>> GetSOTypePg(int pageNum, int pageItem);
        Task<IEnumerable<SOTypeModel>> GetSOTypePgSrch(string searchKey, int pageNum, int pageItem);
        Task<SOTypeModel> GetSOTypeById(string soTypeId);
        Task<bool> SOTypeExists(string soTypeId);
        Task<bool> CreateSOType(SOTypeModel soType);
        Task<bool> UpdateSOType(SOTypeModel soType);
        Task<bool> DeleteSOType(string soTypeId);
    }
}
