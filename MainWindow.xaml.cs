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

        private UIElement TabViewNewTab(TabView sender, String header, String content = "")
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
                //IsClosable = true
            };
            var textBox = ((Newtab.Content as ScrollViewer).Content as TextBox);
            textBox.Text = content;
            Debug.WriteLine($"File Content:\n{((Newtab.Content as ScrollViewer).Content as TextBox).Text.ToString()}");
            tabview.TabItems.Add(Newtab);
            tabview.SelectedItem = Newtab;
            SetIsTextModified(textBox, false);
            SetIsTextSaved(textBox, false);
            // Optionally subscribe to the TextChanged event to track modifications
            textBox.TextChanged += (s, e) => { SetIsTextModified(textBox, true); };
            return Newtab;
        }

        private async void OpenExistingFile(object sender, RoutedEventArgs e) {
            var picker = new FileOpenPicker();
            var hwnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(picker, hwnd);

            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".txt");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null) {
                string fileContent = await FileIO.ReadTextAsync(file);
                TabViewItem openTab = (TabViewItem)TabViewNewTab(tabview, file.Name, fileContent);
                openTab.Tag = file.Path;
                SetIsTextSaved(((openTab.Content as ScrollViewer).Content as TextBox), true);
            }
        }
        private async void SaveAsFile(TabViewItem saveTab) {
            if (saveTab == null)
                return;

            // Get the TextBox inside the saveTab
            var textBox = ((saveTab.Content as ScrollViewer)?.Content as TextBox);
            if (textBox == null)
                return;

            // File Save Picker
            var picker = new FileSavePicker();
            var hwnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(picker, hwnd);

            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
            picker.SuggestedFileName = saveTab.Header.ToString();

            // Open save file dialog
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null) {
                // Write the TextBox content to file
                await FileIO.WriteTextAsync(file, textBox.Text);

                // Update the tab (if needed, to mark it as saved)
                saveTab.Header = file.Name; // Update Tab title
                saveTab.Tag = file.Path; // Update Tab tag
                SetIsTextModified(textBox, false); // Reset modification flag if using a dependency property
            }
        }
        private async void Save(TabViewItem saveTab) {
            if (saveTab == null) {
                return;
            }
            var textBox = ((saveTab.Content as ScrollViewer)?.Content as TextBox);
            if(textBox == null) {
                return;
            }

            if (saveTab.Tag is string filePath && !string.IsNullOrEmpty(filePath)) {
                try {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);

                    // Write the updated text to the file
                    await FileIO.WriteTextAsync(file, textBox.Text);
                    Debug.WriteLine($"File updated: {filePath}");

                    // Mark tab as not modified (if using a flag)
                    SetIsTextModified(textBox, false);
                } catch (Exception ex) {
                    Debug.WriteLine($"Error updating file: {ex.Message}");
                }
            }
        }

        private void SaveExistingFile(TabViewItem saveTab) {
            if (saveTab == null) {
                return;
            }

            var textBox = ((saveTab.Content as ScrollViewer)?.Content as TextBox);
            if (!GetIsTextSaved(textBox)) {
                SaveAsFile(saveTab);
                SetIsTextSaved(textBox, true);
            } else if (GetIsTextModified(textBox)) {
                Save(saveTab);
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

        private XamlRoot? mainRoot;
        private MenuFlyout? fileMenuFlyout;
        private MenuFlyout? editMenuFlyout;
        private MenuFlyout? viewMenuFlyout;

        // Define a custom dependency property to track text modification
        public static readonly DependencyProperty IsTextModifiedProperty =
            DependencyProperty.RegisterAttached(
                "IsTextModified", typeof(bool), typeof(MainWindow),
                new PropertyMetadata(false));

        public static void SetIsTextModified(UIElement element, bool value) {
            element.SetValue(IsTextModifiedProperty, value);
        }

        public static bool GetIsTextModified(UIElement element) {
            return (bool)element.GetValue(IsTextModifiedProperty);
        }
        // Define a custom dependency property to track whether the text has been saved
        public static readonly DependencyProperty IsTextSavedProperty =
            DependencyProperty.RegisterAttached(
                "IsTextSaved", typeof(bool), typeof(MainWindow),
                new PropertyMetadata(false));

        public static void SetIsTextSaved(UIElement element, bool value) {
            element.SetValue(IsTextSavedProperty, value);
        }

        public static bool GetIsTextSaved(UIElement element) {
            return (bool)element.GetValue(IsTextSavedProperty);
        }

        // Event handlers for the TabView
        private void tabview_AddTabButtonClick(TabView sender, object args) {
            TabViewNewTab(sender, "Untitled");
        }

        private void tabview_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args) {
            sender.TabItems.Remove(args.Tab);
            if(sender.TabItems.Count == 0) {
                this.Close();
            }
        }

        // Event handler for the MainWindow
        private void onNewTabClick(object sender, RoutedEventArgs e) { TabViewNewTab(tabview, "Untitled"); }

        private void onNewWindowClick(object sender, RoutedEventArgs e) {
            Window newWindow = new MainWindow();
            App.ActiveWindows.Add(newWindow);
            newWindow.Activate();
        }

        private void onOpenClick(object sender, RoutedEventArgs e) { OpenExistingFile(sender, e); }

        private void onSaveClick(object sender, RoutedEventArgs e) { SaveExistingFile((TabViewItem)tabview.SelectedItem); }

        private void onSaveAsClick(object sender, RoutedEventArgs e) { SaveAsFile((TabViewItem)tabview.SelectedItem); }

        private void onSaveAllClick(object sender, RoutedEventArgs e) {
            if (tabview == null) {
                return;
            }
            foreach (TabViewItem tab in tabview.TabItems) {
                SaveExistingFile(tab);
            }
        }

        private void onPageSetUpClick(object sender, RoutedEventArgs e) {

        }

        private void onPrintClick(object sender, RoutedEventArgs e) {

        }

        private void onCloseTabClick(object sender, RoutedEventArgs e) {
            tabview.TabItems.Remove(tabview.TabItems.LastOrDefault());
            if (tabview.TabItems.Count == 0) {
                this.Close();
            }
        }

        private void onCloseWindowClick(object sender, RoutedEventArgs e) { this.Close(); }

        private void onExitClick(object sender, RoutedEventArgs e) {
            foreach (var window in App.ActiveWindows) {
                window.Close();
            }

            Application.Current.Exit();
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
