using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using SDNCast.Extensions;
using SDNCast.Models;
using SDNCast.Services;

namespace SDNCast.Pages
{
    [Authorize]
    public class AdminComponentBase : ComponentBase
    {
        internal AdminModel AdminModel { get; set; } = new AdminModel();

        internal EditContext editContext;

        [Inject]
        private IWebHostEnvironment Environment { get; set; }

        [Inject]
        private ILiveShowDetailsService LiveShowDetailsService { get; set; }

        // [Inject]
        // private IMemoryCache MemoryCache { get; set; }

        [Inject]
        private IOptions<AppSettings> AppSettings { get; set; }

        [Inject]
        private IObjectMapper Mapper { get; set; }

        //[Inject]
        //private ILogger<AdminComponentBase> Logger { get; set; }

        //[Inject]
        //private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        //[Inject]
        //NavigationManager NavigationManager { get; set; }


        [TempData]
        public string SuccessMessage { get; set; }
        public bool ShowSuccessMessage => !string.IsNullOrEmpty(SuccessMessage);

        public string SuccessMessageClear { get; set; }
        public bool ShowSuccessMessageClear => !string.IsNullOrEmpty(SuccessMessageClear);

        protected override async Task OnInitializedAsync()
        {
            editContext = new EditContext(AdminModel);

            // var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            // var user = authState.User;

            // TODO:
            // My guess is that you are always authenticated when you are here ...

            //if (!user.Identity.IsAuthenticated)
            //{
            // TODO: Redirect away to signin

            // NavigationManager.NavigateTo("/about");
            //    return;

            // return Redirect($"/signin");
            //}

            var liveShowDetails = await LiveShowDetailsService.LoadAsync();

            UpdateModelProperties(liveShowDetails);

            //return Page();
        }

        protected void ClearCache()
        {
            LiveShowDetailsService.ClearCache();

            //MemoryCache.Remove(AdminModel.AppSettings.YouTubeLiveEventsPlaylistId);
            //MemoryCache.Remove(AdminModel.AppSettings.YouTubeCastPlaylistId);

            SuccessMessageClear = "YouTube cache cleared successfully!";
        }


        protected async Task HandleSubmit()
        {
            var isValid = editContext.Validate() &&
                ServerValidate();

            if (isValid)
            {
                var liveShowDetails = await LiveShowDetailsService.LoadAsync();
                Mapper.Map(AdminModel, liveShowDetails);
                await LiveShowDetailsService.SaveAsync(liveShowDetails);

                SuccessMessage = "Live show details saved successfully!";
            }
            else
            {
                //
            }
        }

        private bool ServerValidate()
        {
            return true;
        }

        protected async void HandleValidSubmit()
        {
            // var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            // var user = authState.User;

            // Already authenticated?
            //if (!user.Identity.IsAuthenticated)
            //{
            //    //    return Unauthorized();
            //}

            var liveShowDetails = await LiveShowDetailsService.LoadAsync();

            // Replace insecure http protocol with https
            if (!string.IsNullOrEmpty(AdminModel.LiveShowEmbedUrl) &&
                AdminModel.LiveShowEmbedUrl.StartsWith("http://"))
            {
                AdminModel.LiveShowEmbedUrl = "https://" + AdminModel.LiveShowEmbedUrl.Substring("http://".Length);
            }

            //TrackShowEvent(input, liveShowDetails);

            Mapper.Map(AdminModel, liveShowDetails);
            liveShowDetails.NextShowDateUtc = AdminModel.NextShowDateCET?.ConvertFromCetToUtc();

            await LiveShowDetailsService.SaveAsync(liveShowDetails);

            SuccessMessage = "Live show details saved successfully!";
        }

        internal void OnSelectNextThursday()
        {
            AdminModel.NextShowDateCET = Convert.ToDateTime(AdminModel.NextShowDateSuggestionCetPM);
        }

        private void UpdateModelProperties(LiveShowDetailsModel liveShowDetails)
        {
            Mapper.Map(liveShowDetails, AdminModel);
            AdminModel.NextShowDateCET = liveShowDetails?.NextShowDateUtc?.ConvertFromUtcToCet();

            var nextThursday = GetNextThursday();
            AdminModel.NextShowDateSuggestionCetPM = nextThursday.AddHours(20).ToString("MM/dd/yyyy HH:mm");

            AdminModel.AppSettings = AppSettings.Value;
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
