using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIApp1
{
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
            var m_window = new MainWindow();
            ActiveWindows.Add(m_window);
            m_window.Activate();
        }

        private Window? m_window;
        static public List<Window> ActiveWindows { get { return _activeWindows; } }
        static private List<Window> _activeWindows = new List<Window>();

        static public Window? GetWindowForElement(UIElement element) {
            if (element.XamlRoot != null) {
                foreach (Window window in _activeWindows) {
                    if (element.XamlRoot == window.Content.XamlRoot) {
                        return window;
                    }
                }
            }
            return null;
        }
    }
}
