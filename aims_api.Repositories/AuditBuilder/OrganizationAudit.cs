using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace aims_api.Repositories.AuditBuilder
{
    internal class OrganizationAudit
    {
        public async Task<AuditTrailModel> BuildTranAuditADD(OrganizationModel organization)
        {
            string actTyp = "ADD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(organization);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = organization.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = organization.OrganizationId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditMOD(OrganizationModel organization)
        {
            string actTyp = "MOD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(organization);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = organization.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = organization.OrganizationId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(OrganizationModel organization, string userAccountId)
        {
            string actTyp = "DEL";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(organization);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = organization.OrganizationId,
                    Data = data
                };
            });
        }
    }
}
