using Microsoft.AspNetCore.Components;

using SDNCast.Models;

namespace SDNCast.Components
{
    public class ShowThumbnailBase : ComponentBase
    {
        [Parameter]
        public Show Context { get; set; }
    }
}
