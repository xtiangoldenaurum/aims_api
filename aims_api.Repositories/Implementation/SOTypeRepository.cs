using aims_api.Models;
using aims_api.Repositories.Interface;
using aims_api.Utilities.Interface;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class SOTypeRepository : ISOTypeRepository
    {
        private string ConnString;

        public SOTypeRepository(ITenantProvider tenantProvider)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
        }

        public async Task<IEnumerable<SOTypeModel>> GetSOTypePg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from SOType limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<SOTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<SOTypeModel>> GetSOTypePgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from SOType where 
														soTypeId like @searchKey or 
														soTypeName like @searchKey or 
														description like @searchKey or 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<SOTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<SOTypeModel> GetSOTypeById(string soTypeId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from SOType where 
														soTypeId = @soTypeId";

                var param = new DynamicParameters();
				param.Add("@soTypeId", soTypeId);
                return await db.QuerySingleOrDefaultAsync<SOTypeModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> SOTypeExists(string soTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select soTypeId from SOType where 
														soTypeId = @soTypeId";

                var param = new DynamicParameters();
				param.Add("@soTypeId", soTypeId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateSOType(SOTypeModel soType)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into SOType(soTypeId, 
														soTypeName, 
														description)
 												values(@soTypeId, 
														@soTypeName, 
														@description)";

                int res = await db.ExecuteAsync(strQry, soType);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateSOType(SOTypeModel soType)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update SOType set 
														soTypeName = @soTypeName, 
														description = @description where 
														soTypeId = @soTypeId";

                int res = await db.ExecuteAsync(strQry, soType);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteSOType(string soTypeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from SOType where 
														soTypeId = @soTypeId";
                var param = new DynamicParameters();
				param.Add("@soTypeId", soTypeId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
