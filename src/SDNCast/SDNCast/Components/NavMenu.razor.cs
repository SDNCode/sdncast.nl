using Microsoft.AspNetCore.Components;

namespace SDNCast.Components
{
    public class NavMenuBase : ComponentBase
    {
        bool collapseNavMenu = true;

        string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
