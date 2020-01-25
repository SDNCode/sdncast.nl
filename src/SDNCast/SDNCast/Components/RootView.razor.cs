using Microsoft.AspNetCore.Components;

namespace SDNCast.Components
{
    public partial class RootView : ComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
