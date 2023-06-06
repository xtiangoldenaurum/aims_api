using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IPOLineStatusRepository
    {
        Task<IEnumerable<POLineStatusModel>> GetPOLineStatusPg(int pageNum, int pageItem);
        Task<IEnumerable<POLineStatusModel>> GetPOLineStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<POLineStatusModel> GetPOLineStatusById(string poLineStatusId);
        Task<bool> POLineStatusExists(string poLineStatusId);
        Task<bool> CreatePOLineStatus(POLineStatusModel poLineStatus);
        Task<bool> UpdatePOLineStatus(POLineStatusModel poLineStatus);
        Task<bool> DeletePOLineStatus(string poLineStatusId);
    }
}
