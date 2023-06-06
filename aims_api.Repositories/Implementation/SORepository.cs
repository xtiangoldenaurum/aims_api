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
	public class SORepository : ISORepository
	{
		private string ConnString;

		public SORepository(ITenantProvider tenantProvider)
		{
			ConnString = tenantProvider.GetTenant().SqlConnectionString;
		}

		public async Task<IEnumerable<SOModel>> GetSOPg(int pageNum, int pageItem)
		{
			// pagination setup
			int offset = (pageNum - 1) * pageItem;
			using (IDbConnection db = new MySqlConnection(ConnString))
			{
				db.Open();
				string strQry = "select * from SO limit @pageItem offset @offset";

				var param = new DynamicParameters();
				param.Add("@pageItem", pageItem);
				param.Add("@offset", offset);
				return await db.QueryAsync<SOModel>(strQry, param, commandType: CommandType.Text);
			}
		}

		public async Task<IEnumerable<SOModel>> GetSOPgSrch(string searchKey, int pageNum, int pageItem)
		{
			// pagination setup
			int offset = (pageNum - 1) * pageItem;
			using (IDbConnection db = new MySqlConnection(ConnString))
			{
				db.Open();
				string strQry = @"select * from SO where 
														soId like @searchKey or 
														soTypeId like @searchKey or 
														refNumber like @searchKey or 
														refNumber2 like @searchKey or 
														consigneeId like @searchKey or 
														consigneeName like @searchKey or 
														consigneeAddress like @searchKey or 
														consigneeContact like @searchKey or 
														consigneeEmail like @searchKey or 
														supplierId like @searchKey or 
														supplierName like @searchKey or 
														supplierAddress like @searchKey or 
														supplierContact like @searchKey or 
														supplierEmail like @searchKey or 
														carrierId like @searchKey or 
														carrierName like @searchKey or 
														carrierAddress like @searchKey or 
														carrierContact like @searchKey or 
														carrierEmail like @searchKey or 
														orderCreateDate like @searchKey or 
														arrivalDate like @searchKey or 
														arrivalDate2 like @searchKey or 
														soGrossWeight like @searchKey or 
														itemTotalQty like @searchKey or 
														soStatusId like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey or 
														remarks like @searchKey or 
														limit @pageItem offset @offset";

				var param = new DynamicParameters();
				param.Add("@searchKey", $"%{searchKey}%");
				param.Add("@pageItem", pageItem);
				param.Add("@offset", offset);
				return await db.QueryAsync<SOModel>(strQry, param, commandType: CommandType.Text);
			}
		}

		public async Task<SOModel> GetSOById(string soId)
		{
			// pagination setup
			using (IDbConnection db = new MySqlConnection(ConnString))
			{
				db.Open();
				string strQry = @"select * from SO where 
														soId = @soId";

				var param = new DynamicParameters();
				param.Add("@soId", soId);
				return await db.QuerySingleOrDefaultAsync<SOModel>(strQry, param, commandType: CommandType.Text);
			}
		}

		public async Task<bool> SOExists(string soId)
		{
			using (IDbConnection db = new MySqlConnection(ConnString))
			{
				db.Open();
				string strQry = @"select soId from SO where 
														soId = @soId";

				var param = new DynamicParameters();
				param.Add("@soId", soId);

				var res = await db.ExecuteScalarAsync(strQry, param);
				if (res != null && res != DBNull.Value)
				{
					return true;
				}
			}

			return false;
		}

		public async Task<bool> CreateSO(SOModel so)
		{
			using (IDbConnection db = new MySqlConnection(ConnString))
			{
				string strQry = @"insert into SO(soId, 
														soTypeId, 
														refNumber, 
														refNumber2, 
														consigneeId, 
														consigneeName, 
														consigneeAddress, 
														consigneeContact, 
														consigneeEmail, 
														supplierId, 
														supplierName, 
														supplierAddress, 
														supplierContact, 
														supplierEmail, 
														carrierId, 
														carrierName, 
														carrierAddress, 
														carrierContact, 
														carrierEmail, 
														orderCreateDate, 
														arrivalDate, 
														arrivalDate2, 
														soGrossWeight, 
														itemTotalQty, 
														soStatusId, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@soId, 
														@soTypeId, 
														@refNumber, 
														@refNumber2, 
														@consigneeId, 
														@consigneeName, 
														@consigneeAddress, 
														@consigneeContact, 
														@consigneeEmail, 
														@supplierId, 
														@supplierName, 
														@supplierAddress, 
														@supplierContact, 
														@supplierEmail, 
														@carrierId, 
														@carrierName, 
														@carrierAddress, 
														@carrierContact, 
														@carrierEmail, 
														@orderCreateDate, 
														@arrivalDate, 
														@arrivalDate2, 
														@soGrossWeight, 
														@itemTotalQty, 
														@soStatusId, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

				int res = await db.ExecuteAsync(strQry, so);

				if (res > 0)
				{
					return true;
				}
			}

			return false;
		}

		public async Task<bool> UpdateSO(SOModel so)
		{
			using (IDbConnection db = new MySqlConnection(ConnString))
			{
				string strQry = @"update SO set 
														soTypeId = @soTypeId, 
														refNumber = @refNumber, 
														refNumber2 = @refNumber2, 
														consigneeId = @consigneeId, 
														consigneeName = @consigneeName, 
														consigneeAddress = @consigneeAddress, 
														consigneeContact = @consigneeContact, 
														consigneeEmail = @consigneeEmail, 
														supplierId = @supplierId, 
														supplierName = @supplierName, 
														supplierAddress = @supplierAddress, 
														supplierContact = @supplierContact, 
														supplierEmail = @supplierEmail, 
														carrierId = @carrierId, 
														carrierName = @carrierName, 
														carrierAddress = @carrierAddress, 
														carrierContact = @carrierContact, 
														carrierEmail = @carrierEmail, 
														orderCreateDate = @orderCreateDate, 
														arrivalDate = @arrivalDate, 
														arrivalDate2 = @arrivalDate2, 
														soGrossWeight = @soGrossWeight, 
														itemTotalQty = @itemTotalQty, 
														soStatusId = @soStatusId, 
														modifiedBy = @modifiedBy, 
														remarks = @remarks where 
														soId = @soId";

				int res = await db.ExecuteAsync(strQry, so);

				if (res > 0)
				{
					return true;
				}
			}

			return false;
		}

		// place here InUse checker function

		public async Task<bool> DeleteSO(string soId)
		{
			using (IDbConnection db = new MySqlConnection(ConnString))
			{
				string strQry = @"delete from SO where 
														soId = @soId";
				var param = new DynamicParameters();
				param.Add("@soId", soId);
				int res = await db.ExecuteAsync(strQry, param);

				if (res > 0)
				{
					return true;
				}
			}

			return false;
		}

		public async Task<IEnumerable<SOModel>> ExportSO()
		{
			using (IDbConnection db = new MySqlConnection(ConnString))
			{
				db.Open();
				string strQry = "SELECT * FROM so";
				return await db.QueryAsync<SOModel>(strQry, commandType: CommandType.Text);
			}
		}
	}
}
