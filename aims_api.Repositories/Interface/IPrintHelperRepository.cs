    using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IPrintHelperRepository
    {
        Task<IEnumerable<ZPLDetail>> BuildZplDetails(List<BCodeLabelToPrintModel> docsToPrint);
        Task<IEnumerable<BCodeLabelToPrintModel>?> BuildSNZpls(IEnumerable<UniqueTagsModel> serials);
        Task<IEnumerable<BCodeLabelToPrintModel>?> BuildEPCZpls(IEnumerable<UniqueTagsModel> tags);
        Task<IEnumerable<BCodeLabelToPrintModel>?> BuildEPCZplsFromList(List<string> epcList);
    }
}
