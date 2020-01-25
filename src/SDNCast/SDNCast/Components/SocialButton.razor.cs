using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SDNCast.Components
{
    public partial class SocialButton : ComponentBase
    {
        [Parameter]
        public string AccountId { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        internal string Icon { get; set; }
        internal string Url { get; set; }

        internal void NavigateTo()
        {
            // https://github.com/dotnet/aspnetcore/issues/15200
            JsRuntime.InvokeAsync<object>("open", Url, "_blank");
        }
    }
}
