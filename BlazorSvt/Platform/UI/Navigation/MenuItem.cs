using BlazorBootstrap;

namespace BlazorSvt.Platform.UI.Navigation;

public class MenuItem
{
    public string Url { get; set; } = "/";
    public string Text { get; set; } = "";
    public IconName Icon { get; set; }
    public bool Enabled { get; set; } = true;
}