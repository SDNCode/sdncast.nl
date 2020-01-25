using Microsoft.AspNetCore.Components;

namespace SDNCast.Components
{
    public partial class SiteHeader : ComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Title { get; set; }
    }
}
