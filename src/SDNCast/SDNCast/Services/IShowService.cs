using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SDNCast.Services
{
    public interface IShowsService
    {
        Task<ShowList> GetRecordedShowsAsync(ClaimsPrincipal user, bool disableCache, string playlist);
    }
}
