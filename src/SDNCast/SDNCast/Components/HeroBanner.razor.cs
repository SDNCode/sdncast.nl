using Microsoft.AspNetCore.Components;

using SDNCast.Models;

namespace SDNCast.Components
{
    public partial class HeroBanner : ComponentBase
    {
        [Parameter]
        public HeroBannerModel Context { get; set; }
    }
}
