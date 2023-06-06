using aims_api.Models;
using aims_api.Repositories.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class PagingRepository : IPagingRepository
    {
        public async Task<Pagination?> GetPageDetail(IDbConnection db, string strQry, int pageNum, int pageItem, int rowCount)
        {
            // build pagination detail
            int totalRows = await db.ExecuteScalarAsync<int>(strQry);

            int startCount = 0;
            int count = 1;
            if (pageNum > 1)
            {
                int tempCount = pageNum - 1;
                tempCount = (tempCount * pageItem) + 1;
                count = tempCount;
            }

            startCount = count;
            int totalPages = (totalRows / pageItem);
            if ((totalPages * pageItem) < totalRows)
            {
                totalPages += 1;
            }

            return new Pagination()
            {
                Count = totalRows,
                CurrentPage = pageNum,
                PageSize = pageItem,
                TotalPages = totalPages,
                RowFrom = startCount,
                RowTo = (startCount + rowCount) - 1,
            };
        }

        public async Task<Pagination?> GetPageDetail(IDbConnection db, string strQry, DynamicParameters param, int pageNum, int pageItem, int rowCount)
        {
            // build pagination detail
            int totalRows = await db.ExecuteScalarAsync<int>(strQry, param);

            int startCount = 0;
            int count = 1;
            if (pageNum > 1)
            {
                int tempCount = pageNum - 1;
                tempCount = (tempCount * pageItem) + 1;
                count = tempCount;
            }

            startCount = count;
            int totalPages = (totalRows / pageItem);
            if ((totalPages * pageItem) < totalRows)
            {
                totalPages += 1;
            }

            return new Pagination()
            {
                Count = totalRows,
                CurrentPage = pageNum,
                PageSize = pageItem,
                TotalPages = totalPages,
                RowFrom = startCount,
                RowTo = (startCount + rowCount) - 1,
            };
        }
    }
}
