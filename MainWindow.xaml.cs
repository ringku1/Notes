using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Graphics.Printing3D;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIApp1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow() {
            this.InitializeComponent();
            if (this.Content != null) {
                mainRoot = this.Content.XamlRoot;
            }
        }

        private async void ShowInfoDialog(object sender, RoutedEventArgs e) {
            if (mainRoot == null) {
                mainRoot = this.Content.XamlRoot;
            }

            ContentDialog dialog = new ContentDialog {
                Title = "Information",
                Content = "This is a open message.",
                CloseButtonText = "OK",
                XamlRoot = mainRoot
            };

            await dialog.ShowAsync();
        }

        private void fileOnClick(object sender, RoutedEventArgs e) {
            var openItem = new MenuFlyoutItem { Text = "Open" };
            openItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var saveItem = new MenuFlyoutItem { Text = "Save" };
            saveItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var exitItem = new MenuFlyoutItem { Text = "Exit" };
            exitItem.Click += (s, args) => { Application.Current.Exit(); };

            if(menuFlyout == null) {
                menuFlyout = new MenuFlyout();
                menuFlyout.Items.Add(openItem);
                menuFlyout.Items.Add(saveItem);
                menuFlyout.Items.Add(new MenuFlyoutSeparator());
                menuFlyout.Items.Add(exitItem);

            }
            var button = sender as Button;
            menuFlyout.ShowAt(button);
        }
        
        private XamlRoot? mainRoot;
        private MenuFlyout? menuFlyout;
    }
}
