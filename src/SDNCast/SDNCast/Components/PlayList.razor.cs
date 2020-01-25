using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;

using SDNCast.Models;
using SDNCast.Services;

namespace SDNCast.Components
{
    public partial class PlayList : ComponentBase
    {
        [Inject]
        ILiveShowDetailsService LiveShowDetailsService { get; set; }

        [Inject]
        IObjectMapper Mapper { get; set; }

        [Inject]
        IShowsService ShowsService { get; set; }

        //[Inject]
        //IOptions<AppSettings> AppSettings { get; set; }

        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string PlayListId { get; set; }

        internal bool showPreviousShows;
        internal bool showMoreShowsUrl;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var uri = new Uri(NavigationManager.Uri);
            QueryHelpers.ParseQuery(uri.Query).TryGetValue("disableCache", out var disableCacheString);
            bool.TryParse(disableCacheString, out bool disableCache);

            var liveShowDetails = await LiveShowDetailsService.LoadAsync();
            string playlist = PlayListId;
            var showList = await ShowsService.GetRecordedShowsAsync(user, disableCache, playlist);

            Mapper.Map(liveShowDetails, banner);
            Mapper.Map(showList, this);
            showPreviousShows = PreviousShows.Count() > 0;
            showMoreShowsUrl = !string.IsNullOrEmpty(MoreShowsUrl);
        }

        internal HeroBannerModel banner = new HeroBannerModel();

        public IEnumerable<Show> PreviousShows { get; set; }
        public string MoreShowsUrl { get; set; }
    }
}
