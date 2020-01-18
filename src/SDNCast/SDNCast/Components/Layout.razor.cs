using Microsoft.AspNetCore.Components;

namespace SDNCast.Components
{
    public class LayoutBase : ComponentBase
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment PageHeader { get; set; }

        [Parameter]
        public RenderFragment PageBody { get; set; }
    }
}
