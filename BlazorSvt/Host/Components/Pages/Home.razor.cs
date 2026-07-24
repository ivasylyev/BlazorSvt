using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Host.Components.Pages
{
    public partial class Home
    {
        [Inject]
        public ILogger<Home> Logger { get; set; } = default!;

        protected override void OnInitialized()
        {
            Logger.LogInformation("Home page initializing");
        }
    }
}
