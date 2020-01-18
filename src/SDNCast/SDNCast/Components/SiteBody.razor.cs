using Microsoft.AspNetCore.Components;

namespace SDNCast.Components
{
    public class SiteBodyBase : ComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
