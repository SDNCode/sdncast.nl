using System.Collections.Generic;
using SDNCast.Models;
namespace SDNCast.Services
{
    public class ShowList
    {
        public IList<Show> PreviousShows { get; set; }

        public string MoreShowsUrl { get; set; }
    }
}
