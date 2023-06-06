using aims_api.Models;
using aims_api.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using aims_api.Utilities;
using System.Data;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using aims_api.Utilities.Interface;

namespace aims_api.Repositories.Implementation
{
    public class TransactionTypeRepository : ITransactionTypeRepository
    {
        private string ConnString;

        public TransactionTypeRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<TransactionTypeModel>> GetAllTranType()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from transactionType";
                return await db.QueryAsync<TransactionTypeModel>(strQry, commandType: CommandType.Text);
            }
        }
    }
}
