using Microsoft.AspNetCore.Components;

namespace SDNCast.Components
{
    public partial class SiteBody : ComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
