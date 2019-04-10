using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace sdncast.nl.Pages.Event
{
    public class IndexModel : PageModel
    {
        private readonly AppSettings _appSettings;
        private readonly IHostingEnvironment _env;
        private readonly ILogger _logger;

        public IndexModel(
            IHostingEnvironment env,
            IOptions<AppSettings> appSettings,
            ILogger<IndexModel> logger)
        {
            _appSettings = appSettings.Value;
            _env = env;
            _logger = logger;
        }

        public string EnvironmentName { get; set; }

        public AppSettings AppSettings { get; set; }

        public IActionResult OnGet()
        {
            AppSettings = _appSettings;
            EnvironmentName = _env.EnvironmentName;

            return Page();
        }
    }
}
