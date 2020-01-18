using Microsoft.AspNetCore.Components;

using SDNCast.Models;

namespace SDNCast.Components
{
    public class HeroBannerBase : ComponentBase
    {
        [Parameter]
        public HeroBannerModel Context { get; set; }
    }
}
