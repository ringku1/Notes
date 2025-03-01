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
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.Graphics;
using NotePadCopy;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIApp1.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TabViewWindowPage : Page {
        private const string DataIdentifier = "MyTabItem";
        private Win32WindowHelper? win32WindowHelper;
        private Window? tabTearOutWindow = null;
        private Object? type = null;

        public TabViewWindowPage() {
            this.InitializeComponent();
            Loaded += TabViewWindowPage_Loaded;
        }

        public void SetupWindowMinSize(Window window) {
            win32WindowHelper = new Win32WindowHelper(window);
            win32WindowHelper.SetWindowMinMaxSize(new Win32WindowHelper.POINT() { x = 500, y = 300 });
        }

        private void TabViewWindowPage_Loaded(object sender, RoutedEventArgs e) {
            var currentWindow = WindowHelper.GetWindowForElement(this);
            if (currentWindow == null) {
                return;
            }
            currentWindow.ExtendsContentIntoTitleBar = true;
            currentWindow.SetTitleBar(CustomDragRegion);
            CustomDragRegion.MinWidth = 188;
        }

        public void AddTabToTabs(TabViewItem tab) {
            RootTabView.TabItems.Add(tab);
        }

        private void RootTabView_TabTearOutWindowRequested(TabView sender, TabViewTabTearOutWindowRequestedEventArgs args) {
            var newPage = new TabViewWindowPage();
            newPage.SetTypeAndHandle(args);
            tabTearOutWindow = WindowHelper.CreateWindow();
            tabTearOutWindow.ExtendsContentIntoTitleBar = true;
            tabTearOutWindow.Content = newPage;
            newPage.SetupWindowMinSize(tabTearOutWindow);

            args.NewWindowId = tabTearOutWindow.AppWindow.Id;
        }

        private void RootTabView_TabTearOutRequested(TabView sender, TabViewTabTearOutRequestedEventArgs args) {
            if (tabTearOutWindow == null || tabTearOutWindow.Content == null) {
                return;
            }

            var newPage = (TabViewWindowPage)tabTearOutWindow.Content;

            foreach (TabViewItem tab in args.Tabs.Cast<TabViewItem>()) {
                GetParentTabView(tab)?.TabItems.Remove(tab);
                newPage.AddTabToTabs(tab);
            }
        }

        private void RootTabView_ExternalTornOutTabsDropping(TabView sender, TabViewExternalTornOutTabsDroppingEventArgs args) {
            args.AllowDrop = true;
        }

        private void RootTabView_ExternalTornOutTabsDropped(TabView sender, TabViewExternalTornOutTabsDroppedEventArgs args) {
            int position = 0;

            foreach (TabViewItem tab in args.Tabs.Cast<TabViewItem>()) {
                GetParentTabView(tab)?.TabItems.Remove(tab);
                sender.TabItems.Insert(args.DropIndex + position, tab);
                position++;
            }
        }

        private TabView? GetParentTabView(TabViewItem tab) {
            DependencyObject current = tab;

            while (current != null) {
                if (current is TabView tabView) {
                    return tabView;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }

        private TabViewItem CreateNewTVI(TabView sender, string header) {
            TabViewItem? firstTab = sender.TabItems.FirstOrDefault() as TabViewItem;

            // If no tabs exist, create the first one with a new DataContext.
            if (firstTab == null) {
                firstTab = new TabViewItem() {
                    Header = header,
                    Content = AddContent()
                };
                return firstTab;
            }

            // If a first tab exists, create a new one and copy its DataContext
            var newTab = new TabViewItem() {
                Header = header,
                Content = AddContent()
            };
            newTab.Width = firstTab.ActualWidth;
            newTab.Height = firstTab.ActualHeight;
            return newTab;
        }

        private void RootTabView_AddTabButtonClick(TabView sender, object? args) {
            try {
                // Your logic to add a new TabViewItem
                var tab = CreateNewTVI(sender, "Untitled");
                sender.TabItems.Add(tab);
            } catch (Exception ex) {
                Debug.WriteLine($"Error adding TabViewItem: {ex.Message}");
                // Handle exception (e.g., show a message to the user)
            }
        }

        private void RootTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args) {
            sender.TabItems.Remove(args.Tab);

            if (sender.TabItems.Count == 0) {
                var currentWindow = WindowHelper.GetWindowForElement(this);
                if (currentWindow != null) {
                    currentWindow.Close();
                }
            }
        }

        // User Defined Methods
        private UIElement AddContent() {
            return new ScrollViewer {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = new TextBox { Text = "", AcceptsReturn = true, TextWrapping = TextWrapping.Wrap }
            };
        }
        public void SetTypeAndHandle(Object? type) {
            this.type = type;
            HandleEventArgsType();
        }
        private void HandleEventArgsType() {
            if (type == null) {
                RootTabView_AddTabButtonClick(RootTabView, type);
                return;
            }

            // Get the type of the event args dynamically.
            EventType eventType = GetEventTypeFromType(type.GetType());

            switch (eventType) {
                case EventType.TabViewTabTearOutWindowRequestedEventArgs:
                case EventType.TabViewTabTearOutRequestedEventArgs:
                case EventType.TabViewExternalTornOutTabsDroppingEventArgs:
                case EventType.TabViewExternalTornOutTabsDroppedEventArgs:
                case EventType.TabViewTabCloseRequestedEventArgs:
                    // Handle the event args.
                    // Do nothing for now.
                    break;
            }
        }
        // Helper method to convert event type to EventType enum.
        public static EventType GetEventTypeFromType(Type eventType) {
            // Map the type name to the corresponding EventType.
            return eventType.Name switch {
                nameof(TabViewTabTearOutWindowRequestedEventArgs) => EventType.TabViewTabTearOutWindowRequestedEventArgs,
                nameof(TabViewTabTearOutRequestedEventArgs) => EventType.TabViewTabTearOutRequestedEventArgs,
                nameof(TabViewExternalTornOutTabsDroppingEventArgs) => EventType.TabViewExternalTornOutTabsDroppingEventArgs,
                nameof(TabViewExternalTornOutTabsDroppedEventArgs) => EventType.TabViewExternalTornOutTabsDroppedEventArgs,
                nameof(TabViewTabCloseRequestedEventArgs) => EventType.TabViewTabCloseRequestedEventArgs,
                _ => EventType.Unknown
            };
        }
        public enum EventType {
            TabViewTabTearOutWindowRequestedEventArgs,
            TabViewTabTearOutRequestedEventArgs,
            TabViewExternalTornOutTabsDroppingEventArgs,
            TabViewExternalTornOutTabsDroppedEventArgs,
            TabViewTabCloseRequestedEventArgs,
            Unknown
        }
    }
}
