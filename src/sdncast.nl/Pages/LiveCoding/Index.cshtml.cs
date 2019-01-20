// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using sdncast.nl.Models;
using sdncast.nl.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace sdncast.nl.Pages.LiveCoding
{
    public class IndexModel : PageModel
    {
        private static readonly string _dateTimeFormat = "yyyyMMddTHHmmssZ";
        private static readonly string _googleCalendarText = UrlEncoder.Default.Encode("SDN Cast");
        private static readonly string _googleCalendarLocation = UrlEncoder.Default.Encode("https://sdncast.nl/");

        private readonly ILiveShowDetailsService _liveShowDetails;
        private readonly IShowsService _showsService;
        private readonly IObjectMapper _mapper;
        private readonly AppSettings _appSettings;

        public IndexModel(
            IShowsService showsService, 
            ILiveShowDetailsService liveShowDetails,
            IOptions<AppSettings> appSettings,
            IObjectMapper mapper)
        {
            _showsService = showsService;
            _liveShowDetails = liveShowDetails;
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }

        public bool IsOnAir => !HasAdminMessage && (IsLiveShowEmbedded || !string.IsNullOrEmpty(LiveShowHtml));

        public string LiveShowEmbedUrl { get; set; }

        public string LiveShowHtml { get; set; }

        public bool IsLiveShowEmbedded => !string.IsNullOrEmpty(LiveShowEmbedUrl);

        public DateTime? NextShowDateUtc { get; set; }

        public bool NextShowScheduled => NextShowDateUtc.HasValue;

        public string AdminMessage { get; set; }

        public bool HasAdminMessage => !string.IsNullOrEmpty(AdminMessage);

        public IList<Show> PreviousShows { get; set; }

        public bool ShowPreviousShows => PreviousShows.Count > 0;

        public string MoreShowsUrl { get; set; }

        public bool ShowMoreShowsUrl => !string.IsNullOrEmpty(MoreShowsUrl);

        public string AddToGoogleUrl
        {
            get
            {
                // reference: http://stackoverflow.com/a/21653600/22941
                var from = UrlEncoder.Default.Encode(NextShowDateUtc?.ToString(_dateTimeFormat));
                var to = UrlEncoder.Default.Encode(NextShowDateUtc?.AddMinutes(30).ToString(_dateTimeFormat));

                return $"https://www.google.com/calendar/render?action=TEMPLATE&text={_googleCalendarText}&dates={from}/{to}&details={_googleCalendarLocation}&location={_googleCalendarLocation}&sf=true&output=xml";
            }
        }

        public async Task OnGetAsync(bool? disableCache)
        {
            var liveShowDetails = await _liveShowDetails.LoadAsync();
            string playlist = _appSettings.YouTubeLiveEventsPlaylistId;
            var showList = await _showsService.GetRecordedShowsAsync(User, disableCache ?? false, playlist);

            _mapper.Map(liveShowDetails, this);
            _mapper.Map(showList, this);
        }
    }
}
