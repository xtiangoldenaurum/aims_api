using aims_api.Models;
using Dapper;
using System.Data;

namespace aims_api.Repositories.Interface
{
    public interface IPagingRepository
    {
        Task<Pagination?> GetPageDetail(IDbConnection db, string strQry, int pageNum, int pageItem, int rowCount);
        Task<Pagination?> GetPageDetail(IDbConnection db, string strQry, DynamicParameters param, int pageNum, int pageItem, int rowCount);
    }
}
