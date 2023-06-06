using aims_api.Models;
using aims_api.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Audit
{
    public class AreaAudit
    {
        IAuditTrailRepository AuditTrailRepo;
        public AreaAudit(IAuditTrailRepository autidTrailRepo)
        {
            AuditTrailRepo = autidTrailRepo;
        }

        public async Task CreateTranAudit(AreaModel area)
        {
            string actTyp = "ADD";
            string tranTyp = "CONFIG";
            string data = $"Created Area: {area.AreaName}, Desc.: {area.Description}";

            var audit = new AuditTrailModel() { AuditDate = DateTime.Now, 
                                                UserAccountId = area.CreatedBy, 
                                                ActionTypeId = actTyp, 
                                                TransactionTypeId = tranTyp,
                                                Data = data };

            var res = await AuditTrailRepo.CreateAuditTrail(audit);
        }

        public async Task UpdateTranAudit(AreaModel area)
        {
            string actTyp = "MOD";
            string tranTyp = "CONFIG";
            string data = $"Updated AreaId: {area.AreaId}, Area: {area.AreaName}, Desc.: {area.Description}";

            var audit = new AuditTrailModel()
            {
                AuditDate = DateTime.Now,
                UserAccountId = area.ModifiedBy,
                ActionTypeId = actTyp,
                TransactionTypeId = tranTyp,
                RecordId = area.AreaId,
                Data = data
            };

            var res = await AuditTrailRepo.CreateAuditTrail(audit);
        }

        public async Task DeleteTranAudit(string userAccId, AreaModel area)
        {
            string actTyp = "DEL";
            string tranTyp = "CONFIG";
            string data = $"Deleted AreaId: {area.AreaId}, Area: {area.AreaName}, Desc.: {area.Description}";

            var audit = new AuditTrailModel()
            {
                AuditDate = DateTime.Now,
                UserAccountId = userAccId,
                ActionTypeId = actTyp,
                TransactionTypeId = tranTyp,
                Data = data
            };

            var res = await AuditTrailRepo.CreateAuditTrail(audit);
        }
    }
}
