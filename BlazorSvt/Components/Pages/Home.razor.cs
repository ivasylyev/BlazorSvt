using Microsoft.AspNetCore.Components;
using BlazorSvt.Services.Shared;

namespace BlazorSvt.Components.Pages
{
    public partial class Home
    {
        [Inject]
        public ILogger<Home> Logger { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            Logger.LogInformation("Home page initializing");
            await Task.CompletedTask;
        }
    }
}
