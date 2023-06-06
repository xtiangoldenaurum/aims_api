using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface ISOStatusRepository
    {
        Task<IEnumerable<SOStatusModel>> GetSOStatusPg(int pageNum, int pageItem);
        Task<IEnumerable<SOStatusModel>> GetSOStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<SOStatusModel> GetSOStatusById(string soStatusId);
        Task<bool> SOStatusExists(string soStatusId);
        Task<bool> CreateSOStatus(SOStatusModel soStatus);
        Task<bool> UpdateSOStatus(SOStatusModel soStatus);
        Task<bool> DeleteSOStatus(string soStatusId);
    }
}
