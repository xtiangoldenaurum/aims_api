using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IShippingLoadStatusCore
    {
        Task<RequestResponse> GetShippingLoadStatusPg(int pageNum, int pageItem);
        Task<RequestResponse> GetShippingLoadStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetShippingLoadStatusById(string shippingLoadStatusId);
        Task<RequestResponse> CreateShippingLoadStatus(ShippingLoadStatusModel shippingLoadStatus);
        Task<RequestResponse> UpdateShippingLoadStatus(ShippingLoadStatusModel shippingLoadStatus);
        Task<RequestResponse> DeleteShippingLoadStatus(string shippingLoadStatusId);
    }
}
