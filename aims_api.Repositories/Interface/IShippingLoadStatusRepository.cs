using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IShippingLoadStatusRepository
    {
        Task<IEnumerable<ShippingLoadStatusModel>> GetShippingLoadStatusPg(int pageNum, int pageItem);
        Task<IEnumerable<ShippingLoadStatusModel>> GetShippingLoadStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<ShippingLoadStatusModel> GetShippingLoadStatusById(string shippingLoadStatusId);
        Task<bool> ShippingLoadStatusExists(string shippingLoadStatusId);
        Task<bool> CreateShippingLoadStatus(ShippingLoadStatusModel shippingLoadStatus);
        Task<bool> UpdateShippingLoadStatus(ShippingLoadStatusModel shippingLoadStatus);
        Task<bool> DeleteShippingLoadStatus(string shippingLoadStatusId);
    }
}
