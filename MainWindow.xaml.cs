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
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIApp1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            if (this.Content != null)
            {
                mainRoot = this.Content.XamlRoot;
            }

        }

        private async void ShowInfoDialog(object sender, RoutedEventArgs e)
        {
            if (mainRoot == null)
            {
                mainRoot = this.Content.XamlRoot;
            }

            ContentDialog dialog = new ContentDialog
            {
                Title = "Information",
                Content = "This is a open message.",
                CloseButtonText = "OK",
                XamlRoot = mainRoot
            };

            await dialog.ShowAsync();
        }

        private void TabViewNewTab(TabView sender, String header, String content = "")
        {
            Debug.WriteLine($"Original Content:\n{content}");
            var Newtab = new TabViewItem {
                Header = header,
                Content = new ScrollViewer {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Content = new TextBox {
                        AcceptsReturn = true, 
                        TextWrapping = TextWrapping.Wrap, 
                        HorizontalAlignment = HorizontalAlignment.Stretch, 
                        VerticalAlignment = VerticalAlignment.Stretch 
                    }
                },
                IsClosable = true
            };
            var textBox = ((Newtab.Content as ScrollViewer).Content as TextBox);
            textBox.Text = content;
            Debug.WriteLine($"File Content:\n{((Newtab.Content as ScrollViewer).Content as TextBox).Text.ToString()}");
            tabview.TabItems.Add(Newtab);
        }

        private async void OpenFilePicker_Click(object sender, RoutedEventArgs e) {
            var picker = new FileOpenPicker();
            var hwnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(picker, hwnd);

            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".txt");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null) {
                string fileContent = await FileIO.ReadTextAsync(file);
                TabViewNewTab(tabview, file.Name, fileContent);
            }
        }

        private async void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected tab
            if (tabview.SelectedItem is Microsoft.UI.Xaml.Controls.TabViewItem selectedTab)
            {
                // Get the TextBox inside the tab
                if (selectedTab.Content is ScrollViewer scrollViewer && scrollViewer.Content is TextBox textBox)
                {
                    string fileContent = textBox.Text;
                    string fileName = selectedTab.Header.ToString();

                    StorageFile file;

                    if (fileName == "Untitled")
                    {
                        // If it's "Untitled", ask user where to save
                        var savePicker = new FileSavePicker();
                        var hwnd = WindowNative.GetWindowHandle(this);
                        InitializeWithWindow.Initialize(savePicker, hwnd);

                        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                        savePicker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
                        savePicker.SuggestedFileName = "NewFile";

                        file = await savePicker.PickSaveFileAsync();
                        if (file == null) return; // User canceled the save operation

                        // Update the tab name
                        selectedTab.Header = file.Name;
                    }
                    else
                    {
                        // If file has a name, get it from local storage
                        file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                    }

                    // Save the file
                    await FileIO.WriteTextAsync(file, fileContent);
                }
            }
        }

        private void fileOnClick(object sender, RoutedEventArgs e)
        {
            var newTabItem = new MenuFlyoutItem { Text = "New Tab", Width = 250 };
            newTabItem.Click += (s, args) => { TabViewNewTab(tabview, "Untitled"); };

            var newWindowItem = new MenuFlyoutItem { Text = "New Window", Width = 250 };
            newWindowItem.Click += (s, args) =>
            {
                Window newWindow = new MainWindow();
                App.ActiveWindows.Add(newWindow);
                newWindow.Activate();
            };

            var openItem = new MenuFlyoutItem { Text = "Open", Width = 250 };
            openItem.Click += (s, args) => { OpenFilePicker_Click(s, args); };

            var saveItem = new MenuFlyoutItem { Text = "Save", Width = 250 };
            saveItem.Click += (s, args) => { SaveFile_Click(s, args); };

            var saveAsItem = new MenuFlyoutItem { Text = "Save as", Width = 250 };
            saveAsItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var saveAllItem = new MenuFlyoutItem { Text = "Save all", Width = 250 };
            saveAllItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var pageSetupItem = new MenuFlyoutItem { Text = "Page setup", Width = 250 };
            pageSetupItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var printItem = new MenuFlyoutItem { Text = "Print", Width = 250 };
            printItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var closeTabItem = new MenuFlyoutItem { Text = "Close tab", Width = 250 };
            closeTabItem.Click += (s, args) => {
                tabview.TabItems.Remove(tabview.TabItems.LastOrDefault());
                if(tabview.TabItems.Count == 0) {
                    this.Close();
                }
            };

            var closeWindowItem = new MenuFlyoutItem { Text = "Close window", Width = 250 };
            closeWindowItem.Click += (s, args) => { this.Close(); };

            var exitItem = new MenuFlyoutItem { Text = "Exit", Width = 250 };
            exitItem.Click += (s, args) => { CloseAllWindows(); };

            if (fileMenuFlyout == null)
            {
                fileMenuFlyout = new MenuFlyout();
                fileMenuFlyout.Items.Add(newTabItem);
                fileMenuFlyout.Items.Add(newWindowItem);
                fileMenuFlyout.Items.Add(openItem);
                fileMenuFlyout.Items.Add(saveItem);
                fileMenuFlyout.Items.Add(saveAllItem);
                fileMenuFlyout.Items.Add(new MenuFlyoutSeparator());
                fileMenuFlyout.Items.Add(pageSetupItem);
                fileMenuFlyout.Items.Add(printItem);
                fileMenuFlyout.Items.Add(new MenuFlyoutSeparator());
                fileMenuFlyout.Items.Add(closeTabItem);
                fileMenuFlyout.Items.Add(closeWindowItem);
                fileMenuFlyout.Items.Add(exitItem);
            }

            var button = sender as Button;
            var transform = button.TransformToVisual(null);
            var point = transform.TransformPoint(new Point(0, button.ActualHeight));
            fileMenuFlyout.ShowAt(button, point);
        }

        private void editOnClick(object sender, RoutedEventArgs e)
        {
            var undoItem = new MenuFlyoutItem { Text = "Undo", Width = 250 };
            undoItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var cutItem = new MenuFlyoutItem { Text = "Cut", Width = 250 };
            cutItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var copyItem = new MenuFlyoutItem { Text = "Copy", Width = 250 };
            copyItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var pasteItem = new MenuFlyoutItem { Text = "Paste", Width = 250 };
            pasteItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var deleteItem = new MenuFlyoutItem { Text = "Delete", Width = 250 };
            deleteItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var findItem = new MenuFlyoutItem { Text = "Find", Width = 250 };
            findItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var findNextItem = new MenuFlyoutItem { Text = "Find next", Width = 250 };
            findNextItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var findPrevItem = new MenuFlyoutItem { Text = "Find previous", Width = 250 };
            findPrevItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var replaceItem = new MenuFlyoutItem { Text = "Replace", Width = 250 };
            replaceItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var gotoItem = new MenuFlyoutItem { Text = "Goto", Width = 250 };
            gotoItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var selectAllItem = new MenuFlyoutItem { Text = "Select all", Width = 250 };
            selectAllItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var fontItem = new MenuFlyoutItem { Text = "Font", Width = 250 };
            fontItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            if (editMenuFlyout == null)
            {
                editMenuFlyout = new MenuFlyout();
                editMenuFlyout.Items.Add(undoItem);
                editMenuFlyout.Items.Add(new MenuFlyoutSeparator());
                editMenuFlyout.Items.Add(cutItem);
                editMenuFlyout.Items.Add(copyItem);
                editMenuFlyout.Items.Add(pasteItem);
                editMenuFlyout.Items.Add(deleteItem);
                editMenuFlyout.Items.Add(new MenuFlyoutSeparator());
                editMenuFlyout.Items.Add(findItem);
                editMenuFlyout.Items.Add(findNextItem);
                editMenuFlyout.Items.Add(findPrevItem);
                editMenuFlyout.Items.Add(replaceItem);
                editMenuFlyout.Items.Add(gotoItem);
                editMenuFlyout.Items.Add(new MenuFlyoutSeparator());
                editMenuFlyout.Items.Add(selectAllItem);
                editMenuFlyout.Items.Add(new MenuFlyoutSeparator());
                editMenuFlyout.Items.Add(fontItem);
            }

            var button = sender as Button;
            var transform = button.TransformToVisual(null);
            var point = transform.TransformPoint(new Point(-50, button.ActualHeight));
            editMenuFlyout.ShowAt(button, point);
        }
        private void viewOnClick(object sender, RoutedEventArgs e)
        {
            var zoomItem = new MenuFlyoutItem { Text = "Zoom", Width = 250 };
            zoomItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var zoomInItem = new MenuFlyoutItem { Text = "Zoom in", Width = 250 };
            zoomInItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var zoomOutItem = new MenuFlyoutItem { Text = "Zoom out", Width = 250 };
            zoomOutItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var restoreDefZoomItem = new MenuFlyoutItem { Text = "Restore default zoom", Width = 250 };
            restoreDefZoomItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var statusBarItem = new MenuFlyoutItem { Text = "Status bar", Width = 250 }; // these should be checked box
            statusBarItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            var wordWrapItem = new MenuFlyoutItem { Text = "Word wrap", Width = 250 }; // these should be checked box
            wordWrapItem.Click += (s, args) => { ShowInfoDialog(s, args); };

            if (viewMenuFlyout == null)
            {
                viewMenuFlyout = new MenuFlyout();
                viewMenuFlyout.Items.Add(zoomItem);
                viewMenuFlyout.Items.Add(zoomInItem);
                viewMenuFlyout.Items.Add(zoomOutItem);
                viewMenuFlyout.Items.Add(restoreDefZoomItem);
                viewMenuFlyout.Items.Add(new MenuFlyoutSeparator());
                viewMenuFlyout.Items.Add(statusBarItem);
                viewMenuFlyout.Items.Add(wordWrapItem);
            }
            var button = sender as Button;
            var transform = button.TransformToVisual(null);
            var point = transform.TransformPoint(new Point(-100, button.ActualHeight));
            viewMenuFlyout.ShowAt(button, point);
        }
        private void CloseAllWindows()
        {
            foreach (var window in App.ActiveWindows) {
                window.Close();
            }

            Application.Current.Exit();
        }


        private XamlRoot? mainRoot;
        private MenuFlyout? fileMenuFlyout;
        private MenuFlyout? editMenuFlyout;
        private MenuFlyout? viewMenuFlyout;

        private void tabview_AddTabButtonClick(TabView sender, object args) {
            TabViewNewTab(sender, "Untitled");
        }

        private void tabview_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args) {
            sender.TabItems.Remove(args.Tab);
            if(sender.TabItems.Count == 0) {
                this.Close();
            }
        }
    }
}
