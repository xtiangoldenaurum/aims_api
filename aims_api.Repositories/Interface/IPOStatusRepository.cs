using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IPOStatusRepository
    {
        Task<IEnumerable<POStatusModel>> GetPOStatusPg(int pageNum, int pageItem);
        Task<IEnumerable<POStatusModel>> GetPOStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<POStatusModel> GetPOStatusById(string poStatusId);
        Task<bool> POStatusExists(string poStatusId);
        Task<bool> CreatePOStatus(POStatusModel poStatus);
        Task<bool> UpdatePOStatus(POStatusModel poStatus);
        Task<bool> DeletePOStatus(string poStatusId);
    }
}
