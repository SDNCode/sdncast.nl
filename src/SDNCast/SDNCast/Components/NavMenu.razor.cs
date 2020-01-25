using Microsoft.AspNetCore.Components;

namespace SDNCast.Components
{
    public partial class NavMenu : ComponentBase
    {
        bool collapseNavMenu = true;

        string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
