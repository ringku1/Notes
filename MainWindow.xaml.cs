using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
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

        private UIElement TabViewNewTab(String header, String content = "") {
            var textBox = new TextBox {
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap
            };

            var newTab = new TabViewItem {
                Header = header,
                Content = new ScrollViewer {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Content = textBox
                },
            };

            textBox.Text = content;
            SetIsTextModified(textBox, false);
            SetIsTextSaved(textBox, false);

            // Attach TextChanged event to track modifications
            textBox.TextChanged += TextBox_TextChanged;
            
            tabview.TabItems.Add(newTab);
            newTab.Tag = "Untitled.txt"; // Path is not defined
            tabview.SelectedItem = newTab;

            return newTab;
        }

        private async Task OpenExistingFile(object sender, RoutedEventArgs e) {
            var picker = new FileOpenPicker();
            var hwnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(picker, hwnd);

            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".txt");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null) {
                string fileContent = await FileIO.ReadTextAsync(file);
                TabViewItem openTab = (TabViewItem)TabViewNewTab(file.Name, fileContent);
                openTab.Tag = file.Path;
                var textBox = GetChildTextBox(openTab);
                if (textBox != null) {
                    SetIsTextModified(textBox, false);
                    SetIsTextSaved(textBox, true);
                }
            }
        }
        private async Task SaveAsFileAsync(TabViewItem saveTab) {
            Debug.WriteLine($"saveTab Name: {saveTab.Header.ToString()}");
            var textBox = GetChildTextBox(saveTab);
            if (saveTab == null || textBox == null) {
                return;
            }

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

                saveTab.Header = file.Name;
                saveTab.Tag = file.Path;
                SetIsTextModified(textBox, false);
                if (!GetIsTextSaved(textBox))
                    SetIsTextSaved(textBox, true);
            }
        }
        private async Task SaveAsync(TabViewItem saveTab) {
            Debug.WriteLine($"saveTab Name:\n{saveTab.Header.ToString()}");
            var textBox = GetChildTextBox(saveTab);
            if (saveTab == null || textBox == null) {
                return;
            }

            if (saveTab.Tag is string filePath && !string.IsNullOrEmpty(filePath)) {
                try {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);

                    await FileIO.WriteTextAsync(file, textBox.Text);
                    Debug.WriteLine($"File updated: {filePath}");
                    SetIsTextModified(textBox, false);
                } catch (Exception ex) {
                    Debug.WriteLine($"Error updating file: {ex.Message}");
                }
            }
        }

        private async Task SaveExistingFileAsync(TabViewItem saveTab) {
            Debug.WriteLine($"saveTab Name:\n{saveTab.Header.ToString()}");
            var textBox = GetChildTextBox(saveTab);
            if (saveTab == null || textBox == null) {
                return;
            }

            if (!GetIsTextSaved(textBox)) {
                await SaveAsFileAsync(saveTab);
                //SetIsTextSaved(textBox, true);
            } else if (GetIsTextModified(textBox)) {
                await SaveAsync(saveTab);
            } else {
                Debug.WriteLine($"GetIsTextModified(saveTab) = :{GetIsTextModified(saveTab)}");
            }
        }

        private T? GetParent<T>(DependencyObject child) where T : DependencyObject {
            DependencyObject current = child;

            while (current != null) {
                if (current is T parent) {
                    return parent;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }

        private TextBox? GetChildTextBox(TabViewItem parent) {
            if(parent.Content == null)
                return null;
            return (parent.Content as ScrollViewer)?.Content as TextBox;
        }

        // Event handlers for the TabView
        /*private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if(sender is TextBox textBox) {
                if (!GetIsTextModified(textBox)) {
                    SetIsTextModified(textBox, true);
                }
            } else {
                Debug.WriteLine("Caller was unknown.");
            }
        }*/
        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                int totalChars = textBox.Text.Length;
                CounterCharLabel.Text = $"{totalChars} characters"; // Update character count
            }
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                int selectionStart = textBox.SelectionStart;
                string text = textBox.Text;

                // Count newlines before the cursor to determine line number
                int line = text.Take(selectionStart).Count(c => c == '\n') + 1;

                // Find the last newline before the cursor
                int lastNewLine = text.LastIndexOf('\n', Math.Max(0, selectionStart - 1));

                // Column calculation: If no previous newline is found, count from start
                int col = (lastNewLine == -1) ? selectionStart + 1 : selectionStart - lastNewLine;

                // Ensure the column count never goes below 1
                col = Math.Max(col, 1);

                LnColLabel.Text = $"Ln {line}, Col {col}";
            }
        }


        private void tabview_AddTabButtonClick(TabView sender, object args) {
            TabViewNewTab("Untitled");
        }

        private async void tabview_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args) {
            if (GetChildTextBox(args.Tab) is TextBox textBox) {
                if (!GetIsTextSaved(textBox) && textBox.Text == "" || !GetIsTextModified(textBox)) {
                    sender.TabItems.Remove(args.Tab);
                } else {
                    if (mainRoot == null) {
                        mainRoot = this.Content.XamlRoot;
                    }

                    ContentDialog dialog = new ContentDialog {
                        Title = "Notepad",
                        Content = $"Do you want to save changes to {args.Tab.Tag}?",
                        PrimaryButtonText = "Save",
                        SecondaryButtonText = "Don't save",
                        CloseButtonText = "Cancel",
                        XamlRoot = mainRoot
                    };

                    ContentDialogResult result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary) {
                        await SaveExistingFileAsync(args.Tab);
                        if(GetIsTextSaved(textBox))
                            sender.TabItems.Remove(args.Tab);
                    } else if (result == ContentDialogResult.Secondary) {
                        sender.TabItems.Remove(args.Tab);
                    }
                }
            }

            if(sender.TabItems.Count == 0) {
                this.Close();
            }
        }

        // Event handler for the MainWindow
        private void onNewTabClick(object sender, RoutedEventArgs e) { TabViewNewTab("Untitled"); }

        private void onNewWindowClick(object sender, RoutedEventArgs e) {
            Window newWindow = new MainWindow();
            App.ActiveWindows.Add(newWindow);
            newWindow.Activate();
        }

        private async void onOpenClick(object sender, RoutedEventArgs e) { await OpenExistingFile(sender, e); }

        private async void onSaveClick(object sender, RoutedEventArgs e) { await SaveExistingFileAsync((TabViewItem)tabview.SelectedItem); }

        private async void onSaveAsClick(object sender, RoutedEventArgs e) { await SaveAsFileAsync((TabViewItem)tabview.SelectedItem); }

        private async void onSaveAllClick(object sender, RoutedEventArgs e) {
            if (tabview == null) {
                return;
            }
            foreach (TabViewItem tab in tabview.TabItems) {
                await SaveExistingFileAsync(tab);
            }
        }

        private void onPageSetUpClick(object sender, RoutedEventArgs e) {

        }

        private void onPrintClick(object sender, RoutedEventArgs e) {

        }

        private void onCloseTabClick(object sender, RoutedEventArgs e) {
            tabview.TabItems.Remove(tabview.SelectedItem);
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

        // User defined properties
        private XamlRoot? mainRoot;
        
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
    }
}
