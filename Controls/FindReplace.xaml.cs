using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIApp1.Controls
{
    public sealed partial class FindReplace : UserControl {
        public TabViewItem? selectedTab { get; set; }
        public TextBox findBox { get; set; }
        public bool isReplaceToggled = false;
        public List<int> result = new List<int>();
        private int _idx = -1, _prevIdx = -1;
        private string target = "";

        public FindReplace() {
            this.InitializeComponent();
            this.findBox = FindTextBox;
            SharedShadow.Receivers.Add(ShadowReceiver);
            PopupRectangle.Translation = new Vector3(0, 0, 32);
        }
        private void OnToggleButtonClick(object? sender, RoutedEventArgs? e) {
            if (ReplaceSection.Visibility == Visibility.Visible) {
                ChevronIcon.Glyph = "\uE70E";
                ReplaceSection.Visibility = Visibility.Collapsed;
            } else {
                ChevronIcon.Glyph = "\uE70D";
                ReplaceSection.Visibility = Visibility.Visible;
            }
            ToolTipService.SetToolTip(ToggledButton, (ReplaceSection.Visibility == Visibility.Visible? "Close replace options": "Open replace options"));
            isReplaceToggled = !isReplaceToggled;
        }
        public void ToggleReplaceSection() {
            OnToggleButtonClick(null, null);
        }
        private async void OnFindPreviousClick(object sender, RoutedEventArgs e) {
            if (result.Count > 0) {
                _prevIdx = _idx;
                _idx = _idx - 1 + result.Count;
                _idx = _idx % result.Count;
                await ChangeFocusTxt(_idx == result.Count - 1? "Found next from the bottom": "");
            }
        }

        private async void OnFindNextClick(object sender, RoutedEventArgs e) {
            if (result.Count > 0) {
                _prevIdx = _idx;
                _idx += 1;
                _idx = _idx % result.Count;
                await ChangeFocusTxt(_idx == 0 ? "Found next from the top" : "");
            }
        }

        private void OnMoreOptionsClick(object sender, RoutedEventArgs e) {

        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e) {
            this.Visibility = Visibility.Collapsed;
            if(ReplaceSection.Visibility == Visibility.Visible) {
                ToggleReplaceSection();
            }
        }
        private void OnReplaceClick(object sender, RoutedEventArgs e) {

        }

        private void OnReplaceAllClick(object sender, RoutedEventArgs e) {

        }

        private async void OnSearchIconClick(object sender, RoutedEventArgs e) {
            var stFindText = FindTextBox.Text.Trim();
            if (target != stFindText) { 
                target = stFindText;
                FindOccurrence();
            }
            var matches = result;

            if (matches.Count > 0) {
                _prevIdx = _idx;
                _idx += 1;
                _idx = _idx % result.Count;
                await ChangeFocusTxt(_idx == 0 && _prevIdx != -1 ? "Found next from the top" : "");
            } else {
                ContentDialog noMatchDialog = new ContentDialog {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Notepad",
                    Content = $"Cannot find \"{target}\"",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close
                };
                _ = noMatchDialog.ShowAsync();
            }
        }
        private void FindOccurrence() {
            result.Clear();
            if (selectedTab != null) {
                var editor = MainPage.GetChildTextBox(selectedTab);
                if(editor == null) return;

                var editorTxt = editor.Text;
                // Use Regex to find all case-insensitive matches
                var regex = new Regex(Regex.Escape(target), RegexOptions.IgnoreCase | RegexOptions.Compiled);
                var matches = regex.Matches(editorTxt);

                foreach (Match match in matches) {
                    result.Add(match.Index);
                }
            }
        }
        private async Task ChangeFocusTxt(string message) {
            if (selectedTab != null) {
                var editor = MainPage.GetChildTextBox(selectedTab);
                if (editor != null && (-1 < _idx && _idx < result.Count)) {
                    editor.Focus(FocusState.Programmatic);
                    editor.Select(result[_idx], target.Length);
                }
            }
            if(message != "") {
                WrapAroundText.Text = message;
                WrapAroundPopup.IsOpen = true;

                await Task.Delay(1500);
                WrapAroundPopup.IsOpen = false;
            }
        }
    }
}
