using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

using SDNCast.Models;
using SDNCast.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDNCast.Components
{
    public class PlayListBase : ComponentBase
    {
        [Inject]
        ILiveShowDetailsService liveShowDetailsService { get; set; }

        [Inject]
        IObjectMapper mapper { get; set; }

        [Inject]
        IShowsService showsService { get; set; }

        [Inject]
        IOptions<AppSettings> appSettings { get; set; }

        [Inject]
        AuthenticationStateProvider authenticationStateProvider { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string PlayListId { get; set; }

        internal bool showPreviousShows;
        internal bool showMoreShowsUrl;

        protected override async Task OnInitializedAsync()
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var uri = new Uri(NavigationManager.Uri);
            QueryHelpers.ParseQuery(uri.Query).TryGetValue("disableCache", out var disableCacheString);
            bool.TryParse(disableCacheString, out bool disableCache);

            var liveShowDetails = await liveShowDetailsService.LoadAsync();
            string playlist = PlayListId;
            var showList = await showsService.GetRecordedShowsAsync(user, disableCache, playlist);

            mapper.Map(liveShowDetails, banner);
            mapper.Map(showList, this);
            showPreviousShows = PreviousShows.Count() > 0;
            showMoreShowsUrl = !string.IsNullOrEmpty(MoreShowsUrl);
        }

        internal HeroBannerModel banner = new HeroBannerModel();

        public IEnumerable<Show> PreviousShows { get; set; }
        public string MoreShowsUrl { get; set; }
    }
}
