using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SDNCast.Extensions;
using SDNCast.Models;
using SDNCast.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SDNCast.Pages
{
    [Authorize]
    public class AdminComponentBase: ComponentBase
    {
        [Inject]
        private IWebHostEnvironment Environment { get; set; }
        [Inject]
        private ILiveShowDetailsService liveShowDetailsService { get; set; }
        [Inject]
        private IMemoryCache memoryCache { get; set; }
        [Inject]
        private IOptions<AppSettings> appSettings { get; set; }
        [Inject]
        private IObjectMapper mapper { get; set; }
        [Inject]
        private ILogger<AdminComponentBase> logger { get; set; }

        public AdminModel AdminModel { get; set; } = new AdminModel();

        [TempData]
        public string SuccessMessage { get; set; }
        public bool ShowSuccessMessage => !string.IsNullOrEmpty(SuccessMessage);
        protected override async Task OnInitializedAsync()
        {
            //if (!User.Identity.IsAuthenticated)
            //{
            //    return Redirect($"/signin");
            //}
            var liveShowDetails = await liveShowDetailsService.LoadAsync();

            UpdateModelProperties(liveShowDetails);

            //return Page();
        }

        protected void HandleValidSubmit()
        {
            Console.WriteLine("OnValidSubmit");
        }

        private void UpdateModelProperties(LiveShowDetailsModel liveShowDetails)
        {
            mapper.Map(liveShowDetails, AdminModel);
            AdminModel.NextShowDatePst = liveShowDetails?.NextShowDateUtc?.ConvertFromUtcToCet();

            var nextThursday = GetNextThursday();
            AdminModel.NextShowDateSuggestionCetPM = nextThursday.AddHours(20).ToString("MM/dd/yyyy HH:mm");

            AdminModel.AppSettings = appSettings.Value;
            AdminModel.EnvironmentName = Environment.EnvironmentName;
        }

        private DateTime GetNextThursday()
        {
            var nowCet = DateTime.UtcNow.ConvertFromUtcToCet();
            var remainingDays = 7 - ((int)nowCet.DayOfWeek + 3) % 7;
            var nextThursday = nowCet.AddDays(remainingDays);

            return nextThursday.Date;
        }
    }
}
