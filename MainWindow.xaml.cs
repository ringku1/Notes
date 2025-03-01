using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIApp1.Pages;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIApp1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window {
        public MainWindow() {
            this.InitializeComponent();
            this.TabViewFrame.Navigate(typeof(TabViewWindowPage));
            if (TabViewFrame.Content is TabViewWindowPage tabPage) {
                tabPage.SetTypeAndHandle(null);
            }
        }

        private void onNewTabClick(object sender, RoutedEventArgs e) {
        }

        private void onNewWindowClick(object sender, RoutedEventArgs e) {

        }

        private void onOpenClick(object sender, RoutedEventArgs e) {

        }

        private void onSaveClick(object sender, RoutedEventArgs e) {

        }

        private void onSaveAsClick(object sender, RoutedEventArgs e) {

        }

        private void onSaveAllClick(object sender, RoutedEventArgs e) {

        }

        private void onPageSetUpClick(object sender, RoutedEventArgs e) {

        }

        private void onPrintClick(object sender, RoutedEventArgs e) {

        }

        private void onCloseTabClick(object sender, RoutedEventArgs e) {

        }

        private void onCloseWindowClick(object sender, RoutedEventArgs e) {

        }

        private void onExitClick(object sender, RoutedEventArgs e) {

        }

        private void onUndoClick(object sender, RoutedEventArgs e) {

        }

        private void onCutClick(object sender, RoutedEventArgs e) {

        }

        private void onCopyClick(object sender, RoutedEventArgs e) {

        }

        private void onPasteClick(object sender, RoutedEventArgs e) {

        }

        private void onDeleteClick(object sender, RoutedEventArgs e) {

        }

        private void onBingClick(object sender, RoutedEventArgs e) {

        }

        private void onFindClick(object sender, RoutedEventArgs e) {

        }

        private void onFindNextClick(object sender, RoutedEventArgs e) {

        }

        private void onFindPreviousClick(object sender, RoutedEventArgs e) {

        }

        private void onReplaceClick(object sender, RoutedEventArgs e) {

        }

        private void onGoToClick(object sender, RoutedEventArgs e) {

        }

        private void onSelectAllClick(object sender, RoutedEventArgs e) {

        }

        private void onTimeDateClick(object sender, RoutedEventArgs e) {

        }

        private void onFontClick(object sender, RoutedEventArgs e) {

        }

        private void onZoomInClick(object sender, RoutedEventArgs e) {

        }

        private void onZoomOutClick(object sender, RoutedEventArgs e) {

        }

        private void onResDefZoomClick(object sender, RoutedEventArgs e) {

        }

        private void onRewriteClick(object sender, RoutedEventArgs e) {

        }

        private void onMakeShorterClick(object sender, RoutedEventArgs e) {

        }

        private void onMakeLongerClick(object sender, RoutedEventArgs e) {

        }
    }
}
