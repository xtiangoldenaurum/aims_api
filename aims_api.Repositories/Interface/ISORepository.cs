using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface ISORepository
    {
        Task<IEnumerable<SOModel>> GetSOPg(int pageNum, int pageItem);
        Task<IEnumerable<SOModel>> GetSOPgSrch(string searchKey, int pageNum, int pageItem);
        Task<SOModel> GetSOById(string soId);
        Task<bool> SOExists(string soId);
        Task<bool> CreateSO(SOModel so);
        Task<bool> UpdateSO(SOModel so);
        Task<bool> DeleteSO(string soId);
        Task<IEnumerable<SOModel>> ExportSO();
    }
}
