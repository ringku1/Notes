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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIApp1;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class FindPage : Page
{
    public FindPage()
    {
        this.InitializeComponent();
    }

    private void OnToggleButtonClick(object sender, RoutedEventArgs e) {
        if (ReplaceSection.Visibility == Visibility.Visible) {
            ChevronIcon.Glyph = "&#xE70E;";
            ReplaceSection.Visibility = Visibility.Collapsed;
        } else {
            ChevronIcon.Glyph = "&#xE70F;";
            ReplaceSection.Visibility = Visibility.Visible;
        }
    }
    private void OnFindClick(object sender, RoutedEventArgs e) {

    }
    private void OnFindPreviousClick(object sender, RoutedEventArgs e) {

    }

    private void OnFindNextClick(object sender, RoutedEventArgs e) {

    }

    private void OnMoreOptionsClick(object sender, RoutedEventArgs e) {

    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e) {

    }
    private void OnReplaceClick(object sender, RoutedEventArgs e) {

    }

    private void OnReplaceAllClick(object sender, RoutedEventArgs e) {

    }
}
