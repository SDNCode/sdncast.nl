﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using sdncast.nl.Models;
using sdncast.nl.Services;

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace sdncast.nl.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly ILiveShowDetailsService _liveShowDetails;
        private readonly IMemoryCache _memoryCache;
        private readonly AppSettings _appSettings;
        private readonly IHostingEnvironment _env;
        private readonly IObjectMapper _mapper;
        private readonly ILogger _logger;

        public IndexModel(
            IHostingEnvironment env,
            ILiveShowDetailsService liveShowDetails,
            IMemoryCache memoryCache,
            IOptions<AppSettings> appSettings,
            IObjectMapper mapper,
            ILogger<IndexModel> logger)
        {
            _liveShowDetails = liveShowDetails;
            _memoryCache = memoryCache;
            _appSettings = appSettings.Value;
            _env = env;
            _mapper = mapper;
            _logger = logger;
        }

        [Display(Name = "Live Show Embed URL", Description = "URL for embedding the live show")]
        [DataType(DataType.Url)]
        public string LiveShowEmbedUrl { get; set; }

        [Display(Name = "Live Show HTML", Description = "HTML content for the live show")]
        [DataType(DataType.MultilineText)]
        public string LiveShowHtml { get; set; }

        [Display(Name = "Next Show Date/time", Description = "Exact date and time of the next live show in Pacific Time")]
        [DateAfterNow(TimeZoneId = "Central Europe Standard Time")]
        public DateTime? NextShowDatePst { get; set; }

        [Display(Name = "Standby Message", Description = "Message to show on home page during show standby")]
        public string AdminMessage { get; set; }

        public string NextShowDateSuggestionCetPM { get; set; }

        public bool ShowSuccessMessage => !string.IsNullOrEmpty(SuccessMessage);

        public AppSettings AppSettings { get; set; }

        public string EnvironmentName { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect($"/signin");
            }
            var liveShowDetails = await _liveShowDetails.LoadAsync();

            UpdateModelProperties(liveShowDetails);

            return Page();
        }

        [ModelMetadataType(typeof(IndexModel))]
        public class Input
        {
            public string LiveShowEmbedUrl { get; set; }

            public string LiveShowHtml { get; set; }

            public DateTime? NextShowDatePst { get; set; }

            public string AdminMessage { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(Input input)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var liveShowDetails = await _liveShowDetails.LoadAsync() ?? new LiveShowDetails();

            if (!ModelState.IsValid)
            {
                // Model validation error, just return and let the error render
                UpdateModelProperties(liveShowDetails);

                return Page();
            }

            if (!string.IsNullOrEmpty(input.LiveShowEmbedUrl) && input.LiveShowEmbedUrl.StartsWith("http://"))
            {
                input.LiveShowEmbedUrl = "https://" + input.LiveShowEmbedUrl.Substring("http://".Length);
            }

            TrackShowEvent(input, liveShowDetails);

            _mapper.Map(input, liveShowDetails);
            liveShowDetails.NextShowDateUtc = input.NextShowDatePst?.ConvertFromCetToUtc();

            await _liveShowDetails.SaveAsync(liveShowDetails);

            SuccessMessage = "Live show details saved successfully!";

            return RedirectToPage();
        }

        public IActionResult OnPostClearCache()
        {
            _memoryCache.Remove(_appSettings.YouTubeLiveEventsPlaylistId);
            _memoryCache.Remove(_appSettings.YouTubeCastPlaylistId);

            SuccessMessage = "YouTube cache cleared successfully!";

            return RedirectToPage();
        }

        private void UpdateModelProperties(LiveShowDetails liveShowDetails)
        {
            _mapper.Map(liveShowDetails, this);
            NextShowDatePst = liveShowDetails?.NextShowDateUtc?.ConvertFromUtcToCet();

            var nextThursday = GetNextThursday();
            NextShowDateSuggestionCetPM = nextThursday.AddHours(20).ToString("MM/dd/yyyy HH:mm");

            AppSettings = _appSettings;
            EnvironmentName = _env.EnvironmentName;
        }

        private DateTime GetNextThursday()
        {
            var nowCet = DateTime.UtcNow.ConvertFromUtcToCet();
            var remainingDays = 7 - ((int)nowCet.DayOfWeek + 3) % 7;
            var nextThursday = nowCet.AddDays(remainingDays);

            return nextThursday.Date;
        }

        private static EventId _showStarted = new EventId(0, "Show Started");
        private static EventId _showEnded = new EventId(1, "Show Ended");

        private void TrackShowEvent(Input input, LiveShowDetails liveShowDetails)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                var showStarted = string.IsNullOrEmpty(liveShowDetails.LiveShowEmbedUrl) && !string.IsNullOrEmpty(input.LiveShowEmbedUrl);
                var showEnded = !string.IsNullOrEmpty(liveShowDetails.LiveShowEmbedUrl) && string.IsNullOrEmpty(input.LiveShowEmbedUrl);

                if (showStarted)
                {
                    _logger.LogInformation(_showStarted, "Show started streaming at {ShowEmbedUrl}", input.LiveShowEmbedUrl);
                }
                if (showEnded)
                {
                    _logger.LogInformation(_showEnded, "Show ended streaming at {ShowEmbedUrl}", liveShowDetails.LiveShowEmbedUrl);
                }
            }
        }
    }
}
