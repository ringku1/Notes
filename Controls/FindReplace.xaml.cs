using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIApp1.Controls
{
    public sealed partial class FindReplace : UserControl {
        public FindReplace() {
            this.InitializeComponent();
        }
        private void OnToggleButtonClick(object sender, RoutedEventArgs e) {
            if (ReplaceSection.Visibility == Visibility.Visible) {
                ChevronIcon.Glyph = "\uE70E";
                ReplaceSection.Visibility = Visibility.Collapsed;
            } else {
                ChevronIcon.Glyph = "\uE70D";
                ReplaceSection.Visibility = Visibility.Visible;
            }
            ToolTipService.SetToolTip(ToggledButton, (ReplaceSection.Visibility == Visibility.Visible? "Close replace options": "Open replace options"));
        }
        private void OnFindPreviousClick(object sender, RoutedEventArgs e) {

        }

        private void OnFindNextClick(object sender, RoutedEventArgs e) {

        }

        private void OnMoreOptionsClick(object sender, RoutedEventArgs e) {

        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e) {
            this.Visibility = Visibility.Collapsed;
        }
        private void OnReplaceClick(object sender, RoutedEventArgs e) {

        }

        private void OnReplaceAllClick(object sender, RoutedEventArgs e) {

        }

        private void OnSearchIconClick(object sender, RoutedEventArgs e) {

        }
    }
}
