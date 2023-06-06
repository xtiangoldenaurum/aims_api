using aims_api.Models;
using aims_api.Utilities;

namespace aims_api.Cores.Interface
{
    public interface IAreaCore
    {
        Task<RequestResponse> GetAreaSpecial(string? searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetAllArea();
        Task<RequestResponse> GetAreaPg(int pageNum, int pageItem);
        Task<RequestResponse> GetAreaPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetAreaPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetAreaSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetAreaById(string areaId);
        Task<RequestResponse> CreateArea(AreaModel area);
        Task<RequestResponse> UpdateArea(AreaModel area);
        Task<RequestResponse> DeleteArea(string areaId, string userAccountId);
    }
}
