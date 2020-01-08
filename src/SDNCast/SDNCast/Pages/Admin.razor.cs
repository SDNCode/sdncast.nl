using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SDNCast.Extensions;
using SDNCast.Models;
using SDNCast.Services;

using System;
using System.Threading.Tasks;

namespace SDNCast.Pages
{
    [Authorize]
    public class AdminComponentBase : ComponentBase
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
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        public AdminModel AdminModel { get; set; } = new AdminModel();

        [TempData]
        public string SuccessMessage { get; set; }
        public bool ShowSuccessMessage => !string.IsNullOrEmpty(SuccessMessage);

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (!user.Identity.IsAuthenticated)
            {
                // TODO: Redirect away ...

                // NavigationManager.NavigateTo("/");
                // return;
                // return Redirect($"/signin");
            }

            var liveShowDetails = await liveShowDetailsService.LoadAsync();

            UpdateModelProperties(liveShowDetails);

            //return Page();
        }

        async protected void HandleValidSubmit()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (!user.Identity.IsAuthenticated)
            {
                //    return Unauthorized();
            }
            //var liveShowDetails = await _liveShowDetails.LoadAsync() ?? new LiveShowDetails();

            //if (!ModelState.IsValid)
            //{
            //    // Model validation error, just return and let the error render
            //    UpdateModelProperties(liveShowDetails);

            //    return Page();
            //}

            //if (!string.IsNullOrEmpty(input.LiveShowEmbedUrl) && input.LiveShowEmbedUrl.StartsWith("http://"))
            //{
            //    input.LiveShowEmbedUrl = "https://" + input.LiveShowEmbedUrl.Substring("http://".Length);
            //}

            //TrackShowEvent(input, liveShowDetails);

            //_mapper.Map(input, liveShowDetails);
            //liveShowDetails.NextShowDateUtc = input.NextShowDatePst?.ConvertFromCetToUtc();

            //await _liveShowDetails.SaveAsync(liveShowDetails);

            //SuccessMessage = "Live show details saved successfully!";

            //return RedirectToPage();

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
