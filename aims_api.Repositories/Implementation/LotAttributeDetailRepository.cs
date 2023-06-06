using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
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
    public class LotAttributeDetailRepository : ILotAttributeDetailRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        LotAttributeDetailAudit AuditBuilder;

        public LotAttributeDetailRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new LotAttributeDetailAudit();
        }

        public async Task<IEnumerable<LotAttributeDetailModel>> GetLotAttributeDetailPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from LotAttributeDetail limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<LotAttributeDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<LotAttributeDetailModel>> GetLotAttributeDetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from LotAttributeDetail where 
														lotAttributeId like @searchKey or 
														manufactureDate like @searchKey or 
														expiryDate like @searchKey or 
														warehousingDate like @searchKey or 
														productConditionId like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
                                                        modifiedBy like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<LotAttributeDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<LotAttributeDetailModel> GetLotAttributeDetailById(string lotAttributeId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from LotAttributeDetail where 
														lotAttributeId = @lotAttributeId";

                var param = new DynamicParameters();
                param.Add("@lotAttributeId", lotAttributeId);
                return await db.QuerySingleOrDefaultAsync<LotAttributeDetailModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<string> GetLotAttributeIdWithSameDetail(LotAttributeDetailModel lotatt)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select distinct lotAttributeId 
                                    from lotattributedetail 
                                    where date(manufacturedate) = case when @manufacturedate is null then null else date(@manufacturedate) end and 
                                            date(expirydate) = case when @manufacturedate is null then null else date(@expirydate) end and 
                                            date(warehousingdate) = case when @manufacturedate is null then null else date(@warehousingdate) end and 
                                            productconditionid = @productconditionid limit 1";

                var param = new DynamicParameters();
                param.Add("@manufacturedate", lotatt.ManufactureDate);
                param.Add("@expirydate", lotatt.ExpiryDate);
                param.Add("@warehousingdate", lotatt.WarehousingDate);
                param.Add("@productconditionid", lotatt.ProductConditionId);

                return await db.ExecuteScalarAsync<string>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> LotAttributeDetailExists(string lotAttributeId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select lotAttributeId from LotAttributeDetail where 
														lotAttributeId = @lotAttributeId";

                var param = new DynamicParameters();
                param.Add("@lotAttributeId", lotAttributeId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateLotAttributeDetail(LotAttributeDetailModel lotAttributeDetail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"insert into LotAttributeDetail(lotAttributeId, 
														manufactureDate, 
														expiryDate, 
														warehousingDate, 
														productConditionId, 
														createdBy, 
                                                        modifiedBy)
 												values(@lotAttributeId, 
														@manufactureDate, 
														@expiryDate, 
														@warehousingDate, 
														@productConditionId, 
														@createdBy,
                                                        @modifiedBy)";

                int res = await db.ExecuteAsync(strQry, lotAttributeDetail);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADD(lotAttributeDetail, TranType.LOT);

                    if (await AuditTrailRepo.CreateAuditTrail(audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> CreateLotAttributeDetailMod(IDbConnection db, LotAttributeDetailModel lotAttributeDetail, TranType tranTyp)
        {
            string strQry = @"insert into LotAttributeDetail(lotAttributeId, 
														manufactureDate, 
														expiryDate, 
														warehousingDate, 
														productConditionId, 
														createdBy, 
                                                        modifiedBy)
 												values(@lotAttributeId, 
														@manufactureDate, 
														@expiryDate, 
														@warehousingDate, 
														@productConditionId, 
														@createdBy,
                                                        @modifiedBy)";

            int res = await db.ExecuteAsync(strQry, lotAttributeDetail);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditADD(lotAttributeDetail, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateLotAttributeDetail(LotAttributeDetailModel lotAttributeDetail)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"update LotAttributeDetail set 
														manufactureDate = @manufactureDate, 
														expiryDate = @expiryDate, 
														warehousingDate = @warehousingDate, 
														productConditionId = @productConditionId, 
                                                        modifiedBy = @modifiedBy where 
														lotAttributeId = @lotAttributeId";

                int res = await db.ExecuteAsync(strQry, lotAttributeDetail);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(lotAttributeDetail, TranType.LOT);

                    if (await AuditTrailRepo.CreateAuditTrail(audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // place here InUse checker function

        public async Task<bool> DeleteLotAttributeDetail(string lotAttributeId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from LotAttributeDetail where 
														lotAttributeId = @lotAttributeId";
                var param = new DynamicParameters();
                param.Add("@lotAttributeId", lotAttributeId);
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
