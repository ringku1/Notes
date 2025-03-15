using Microsoft.UI.Xaml;
using WinUIApp1.Helpers;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIApp1;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>

public partial class App : Application
{
    public App()
    {
        this.InitializeComponent();
    }
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        m_window = WindowHelper.CreateWindow();
        m_window.Activate();
    }

    private Window? m_window;
}
