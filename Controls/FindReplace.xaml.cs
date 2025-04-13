using System;
using System.Collections.Generic;
using System.Linq;
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
            } else {
                ShowNoMatchDialogue();
            }
        }

        private async void OnFindNextClick(object sender, RoutedEventArgs e) {
            if (sender is Button rep_btn && rep_btn.Name == "ReplaceButton") {
                _idx -= 1;
            }
            if (result.Count > 0) {
                _prevIdx = _idx;
                _idx += 1;
                _idx = _idx % result.Count;
                await ChangeFocusTxt(_idx == 0 ? "Found next from the top" : "");
            } else {
                ShowNoMatchDialogue();
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
            if(_idx != -1 && selectedTab != null && result.Count > 0) {
                var editor = MainPage.GetChildTextBox(selectedTab);
                if (editor != null) {
                    editor.Text = editor.Text.Remove(result[_idx], target.Length).Insert(result[_idx], ReplaceTextBox.Text);
                    result.RemoveAt(_idx);
                    for(int i = _idx; i < result.Count; i++) {
                        result[i] -= Math.Abs(target.Length - ReplaceTextBox.Text.Length);
                    }
                    OnFindNextClick(sender, e);
                }
            } else {
                ShowNoMatchDialogue();
            }
        }

        private void OnReplaceAllClick(object sender, RoutedEventArgs e) {
            if (_idx != -1 && selectedTab != null && result.Count > 0) {
                var editor = MainPage.GetChildTextBox(selectedTab);
                if (editor != null) {
                    string text = editor.Text;
                    // Sort in descending order to avoid index shifting issues
                    var sortedResults = result.ToList();
                    sortedResults.Reverse();

                    foreach (int pos in sortedResults) {
                        if (pos + target.Length <= text.Length) {
                            text = text.Remove(pos, target.Length).Insert(pos, ReplaceTextBox.Text);
                        }
                    }

                    editor.Text = text;
                    result.Clear();
                }
            }
        }
        public void triggerOnSearchIconClick(object sender, RoutedEventArgs e) {
            OnSearchIconClick(sender, e);
        }
        private async void OnSearchIconClick(object sender, RoutedEventArgs e) {
            var stFindText = FindTextBox.Text.Trim();
            if (target != stFindText || result.Count == 0) {
                target = stFindText;
                FindOccurrence();
            }
            var matches = result;
            if(sender is MenuFlyoutItem FindMFI) {
                if (selectedTab != null) {
                    var editor = MainPage.GetChildTextBox(selectedTab);
                    if (editor != null) {
                        _idx = result.FindIndex(pos => pos == editor.SelectionStart);
                        if (_idx != -1) {
                            _idx -= 1;
                        }
                    }
                }
            }

            if (matches.Count > 0) {
                _prevIdx = _idx;
                _idx += 1;
                _idx = _idx % result.Count;
                await ChangeFocusTxt(_idx == 0 && _prevIdx != -1 ? "Found next from the top" : "");
            } else {
                ShowNoMatchDialogue();
            }
        }
        private void ShowNoMatchDialogue() {
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

        private void OnMatchCaseClick(object sender, RoutedEventArgs e) {

        }

        private void OnWrapAroundClick(object sender, RoutedEventArgs e) {

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
