using Microsoft.AspNetCore.Components;

namespace SDNCast.Components
{
    public class SiteHeaderBase : ComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Title { get; set; }
    }
}
