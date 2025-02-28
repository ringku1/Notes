using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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
        }
        private void RootTabView_Loaded(object sender, RoutedEventArgs e) {
            // This handler is invoked when the TabView is loaded

            // Dynamically add tabs
            AddNewTab();
        }
        // Add a new tab to the TabView.
        private void RootTabView_AddTabButtonClick(TabView sender, object args) {
            var existingTab = sender.TabItems.FirstOrDefault() as TabViewItem;
            if (existingTab == null) {
                return;
            }
            var newTab = new TabViewItem();
            newTab.Header = "Untitled";
            newTab.Content = CloneContent(existingTab.Content);
            sender.TabItems.Add(newTab);
            sender.SelectedItem = newTab;
        }
        // Remove the requested tab from the TabView.
        private void RootTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args) {
            sender.TabItems.Remove(args.Tab);
            if (sender.TabItems.Count == 0) {
                this.Close();
            }
        }
        private void AddNewTab() {
            // Add a TabItem programmatically
            if(RootTabView.TabItems.Count > 0) {
                var lastTab = RootTabView.TabItems.LastOrDefault() as TabViewItem;
                if (lastTab != null) {
                    var copyTab = new TabViewItem {
                        Header = lastTab.Header, // Copy Header
                        Content = CloneContent(lastTab.Content) // Clone Content
                    };

                    RootTabView.TabItems.Add(copyTab);
                }
            } else {
                RootTabView.TabItems.Add(new TabViewItem {
                    Header = "Untitled",
                    Content = new ScrollViewer {
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                        Content = new TextBox {
                            AcceptsReturn = true,
                            TextWrapping = TextWrapping.Wrap,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            HorizontalAlignment = HorizontalAlignment.Stretch
                        }
                    }
                });
            }
        }
        private UIElement CloneContent(object content) {
            if (content is ScrollViewer scrollViewer && scrollViewer.Content is TextBox textBox) {
                return new ScrollViewer {
                    VerticalScrollBarVisibility = scrollViewer.VerticalScrollBarVisibility,
                    HorizontalScrollBarVisibility = scrollViewer.HorizontalScrollBarVisibility,
                    Content = new TextBox {
                        AcceptsReturn = textBox.AcceptsReturn,
                        TextWrapping = textBox.TextWrapping,
                        Text = "",
                        VerticalAlignment = textBox.VerticalAlignment,
                        HorizontalAlignment = textBox.HorizontalAlignment
                    }
                };
            }

            return new TextBlock { Text = "Cloned Tab (Unsupported Content Type)" }; // Fallback for unknown content
        }

        private void RootTabView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var selectedTab = RootTabView.SelectedItem as TabViewItem;
            if (selectedTab != null) {
                // Do something when the tab selection changes
                string selectedTabHeader = selectedTab.Header?.ToString() ?? "Unnamed Tab";

                // Example: Display selected tab name
                Debug.WriteLine($"Selected Tab: {selectedTabHeader}");
            }
        }

        private void onNewTabClick(object sender, RoutedEventArgs e) {
            AddNewTab();
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

        private void RootTabView_TabTearOutWindowRequested(TabView sender, TabViewTabTearOutWindowRequestedEventArgs args) {

        }

        private void RootTabView_TabTearOutRequested(TabView sender, TabViewTabTearOutRequestedEventArgs args) {

        }

        private void RootTabView_ExternalTornOutTabsDropping(TabView sender, TabViewExternalTornOutTabsDroppingEventArgs args) {

        }

        private void RootTabView_ExternalTornOutTabsDropped(TabView sender, TabViewExternalTornOutTabsDroppedEventArgs args) {

        }

        /*
* <TabViewItem x:Name="TabViewItem1" Header="Untitled">
  <TabViewItem.Content>
      <ScrollViewer VerticalScrollBarVisibility="Auto" HorizontalScrollBarVisibility="Auto">
          <TextBox 
              Text="" 
              AcceptsReturn="True" 
              TextWrapping="Wrap" 
              VerticalAlignment="Stretch"
              HorizontalAlignment="Stretch"/>
      </ScrollViewer>
  </TabViewItem.Content>
</TabViewItem>
*/
    }
}
