using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using WinUIApp1.Model;
using WinUIApp1.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIApp1;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
    private MainWindow? m_window = null;
    public MainPage()
    {
        this.InitializeComponent();
    }
    protected override void OnNavigatedTo(NavigationEventArgs e) {
        base.OnNavigatedTo(e);
        m_window = e.Parameter as MainWindow;
    }
    private void MainPage_Loaded(object sender, RoutedEventArgs e) {
        this.Loaded -= MainPage_Loaded;
    }

    private async void ShowInfoDialog(object sender, RoutedEventArgs e) {
        ContentDialog dialog = new ContentDialog {
            Title = "Information",
            Content = "This is a open message.",
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
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
            DataContext = new TabData(),
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
        textBox.Loaded += EditorTextBox_Loaded;
        textBox.TextChanged += EditorTextBox_TextChanged;
        textBox.SelectionChanged += EditorTextBox_SelectionChanged;

        tabview.TabItems.Add(newTab);
        newTab.Tag = "Untitled.txt"; // Path is not defined
        tabview.SelectedItem = newTab;

        return newTab;
    }

    private async Task OpenExistingFile(object sender, RoutedEventArgs e) {
        var picker = new FileOpenPicker();
        var hwnd = WindowNative.GetWindowHandle(m_window);
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
        var hwnd = WindowNative.GetWindowHandle(m_window);
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

    static public T? GetParentViaChild<T>(DependencyObject child) where T : DependencyObject {
        DependencyObject current = VisualTreeHelper.GetParent(child);

        while (current != null) {
            if (current is T parent) {
                return parent;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    private TextBox? GetChildTextBox(TabViewItem parent) {
        if (parent.Content == null)
            return null;
        return (parent.Content as ScrollViewer)?.Content as TextBox;
    }
    static public async Task UpdateCursorPosition(TextBox textBox, TabData tabData) {
        string textBeforeCursor = textBox.Text.Substring(0, textBox.SelectionStart);

        int ln = textBeforeCursor.Split(new[] { '\r', '\n' }, StringSplitOptions.None).Length;
        int lastLnIdx = textBeforeCursor.LastIndexOfAny(new char[] { '\r', '\n' });

        string lastLn = (lastLnIdx >= 0) ? textBeforeCursor.Substring(lastLnIdx + 1) : textBeforeCursor;
        int col = lastLn.Length + 1;

        tabData.CursorPosition = new Tuple<int, int>(ln, col);
        tabData.CharacterCount = textBox.Text.Length;
    }

    static public void SetTextBoxCursorPosition(TextBox textBox, Tuple<int, int> cursorPos) {
        int ln = cursorPos.Item1;
        int col = cursorPos.Item2;
        int cursorIdx = 0, lnStartIdx = 0;

        for (int i = 1; i < ln; i++) {
            int nextLineBreak = textBox.Text.IndexOfAny(new char[] { '\r', '\n' }, lnStartIdx);
            if (nextLineBreak == -1)
                break;

            lnStartIdx = nextLineBreak + 1;
        }
        cursorIdx = lnStartIdx + col - 1;

        textBox.SelectionStart = cursorIdx;
        textBox.SelectionLength = 0; // Clear selection
    }
    private void EditorTextBox_Loaded(object sender, RoutedEventArgs e) {
        var textBox = sender as TextBox;
        if (textBox == null)
            return;
        var scrollViewer = GetParentViaChild<ScrollViewer>(textBox);
        if(scrollViewer != null) {
            textBox.Focus(FocusState.Programmatic);
            if (textBox.Text.Length > 0) {
                var rect = textBox.GetRectFromCharacterIndex(0, true);
                scrollViewer.ChangeView(null, rect.Top, null);
            } else {
                scrollViewer.ChangeView(null, 0, null);
            }
        }
    }
    private async void EditorTextBox_TextChanged(object sender, RoutedEventArgs e) {
        if (sender is TextBox textBox && textBox.DataContext is TabData tabData) {
            if (!GetIsTextModified(textBox)) {
                SetIsTextModified(textBox, true);
            }
            if (tabData != null) {
                await UpdateCursorPosition(textBox, tabData);
            }
        }
    }
    private async void EditorTextBox_SelectionChanged(object sender, RoutedEventArgs e) {
        if (sender is TextBox textBox && textBox.DataContext is TabData tabData) {
            if (tabData != null) {
                await UpdateCursorPosition(textBox, tabData);
            }
        }
    }
    private void tabview_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        var selectedTab = tabview.SelectedItem as TabViewItem;
        if (selectedTab != null && selectedTab.DataContext is TabData tabData) {
            if (tabData != null) {
                var textBox = GetChildTextBox(selectedTab);
                if (textBox != null) {
                    SetTextBoxCursorPosition(textBox, tabData.CursorPosition);
                }
            }
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
                ContentDialog dialog = new ContentDialog {
                    Title = "Notepad",
                    Content = $"Do you want to save changes to {args.Tab.Tag}?",
                    PrimaryButtonText = "Save",
                    SecondaryButtonText = "Don't save",
                    CloseButtonText = "Cancel",
                    XamlRoot = this.XamlRoot
                };

                ContentDialogResult result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary) {
                    await SaveExistingFileAsync(args.Tab);
                    if (GetIsTextSaved(textBox))
                        sender.TabItems.Remove(args.Tab);
                } else if (result == ContentDialogResult.Secondary) {
                    sender.TabItems.Remove(args.Tab);
                }
            }
        }

        if (sender.TabItems.Count == 0 && m_window != null) {
            m_window.CloseWindow();
        }
    }

    // Event handler for the MainWindow
    private void onNewTabClick(object sender, RoutedEventArgs e) { TabViewNewTab("Untitled"); }

    private void onNewWindowClick(object sender, RoutedEventArgs e) {
        Window newWindow = WindowHelper.CreateWindow();
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

    private async void onPrintClick(object sender, RoutedEventArgs e)
    {
        
    }

    private void onCloseTabClick(object sender, RoutedEventArgs e) {
        tabview.TabItems.Remove(tabview.SelectedItem);
        if (tabview.TabItems.Count == 0 && m_window != null) {
            m_window.CloseWindow();
        }
    }

    private void onCloseWindowClick(object sender, RoutedEventArgs e) {
        if (m_window != null) {
            m_window.CloseWindow();
        }
    }

    private void onExitClick(object sender, RoutedEventArgs e) {
        while (WindowHelper.ActiveWindows.Count > 0) {
            (WindowHelper.ActiveWindows[0] as MainWindow)?.CloseWindow();
        }

        Application.Current.Exit();
    }

    private void onUndoClick(object sender, RoutedEventArgs e)
    {
        var selectedTab = tabview.SelectedItem as TabViewItem;
        if (selectedTab != null)
        {
            var textBox = GetChildTextBox(selectedTab);
            if (textBox != null)
            {
                var words = textBox.Text.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length > 0)
                {
                    var lastWord = words.Last();
                    var newText = textBox.Text.Substring(0, textBox.Text.LastIndexOf(lastWord));
                    textBox.Text = newText;
                }
            }
        }
    }

    private void onCutClick(object sender, RoutedEventArgs e)
    {
        var selectedTab = tabview.SelectedItem as TabViewItem;
        if (selectedTab != null)
        {
            var textBox = GetChildTextBox(selectedTab);
            if (textBox != null)
            {
                textBox.CutSelectionToClipboard();
            }
        }
    }

    private void onCopyClick(object sender, RoutedEventArgs e)
    {
        var selectedTab = tabview.SelectedItem as TabViewItem;
        if (selectedTab != null)
        {
            var textBox = GetChildTextBox(selectedTab);
            if (textBox != null)
            {
                textBox.CopySelectionToClipboard();
            }
        }
    }

    private void onPasteClick(object sender, RoutedEventArgs e)
    {
        var selectedTab = tabview.SelectedItem as TabViewItem;
        if (selectedTab != null)
        {
            var textBox = GetChildTextBox(selectedTab);
            if (textBox != null)
            {
                textBox.PasteFromClipboard();
            }
        }
    }

    private void onDeleteClick(object sender, RoutedEventArgs e)
    {
        var selectedTab = tabview.SelectedItem as TabViewItem;
        if (selectedTab != null)
        {
            var textBox = GetChildTextBox(selectedTab);
            if (textBox != null)
            {
                var selectionStart = textBox.SelectionStart;
                textBox.Text = textBox.Text.Remove(selectionStart, textBox.SelectionLength);
                textBox.SelectionStart = selectionStart;
            }
        }
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
