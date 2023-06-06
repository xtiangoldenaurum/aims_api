using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface ISOLineStatusRepository
    {
        Task<IEnumerable<SOLineStatusModel>> GetSOLineStatusPg(int pageNum, int pageItem);
        Task<IEnumerable<SOLineStatusModel>> GetSOLineStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<SOLineStatusModel> GetSOLineStatusById(string soLineStatusId);
        Task<bool> SOLineStatusExists(string soLineStatusId);
        Task<bool> CreateSOLineStatus(SOLineStatusModel soLineStatus);
        Task<bool> UpdateSOLineStatus(SOLineStatusModel soLineStatus);
        Task<bool> DeleteSOLineStatus(string soLineStatusId);
    }
}
