using Microsoft.AspNetCore.Components;

namespace SDNCast.Components
{
    public class RootViewBase : ComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
