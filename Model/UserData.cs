using System.ComponentModel;

namespace WinUIApp1.Model {
    public class UserData : INotifyPropertyChanged {
        private string _userName = "User";
        private string _userEmail = "User@example.com";
        private string _profileImage = "Assets/default-pic.png";

        public string UserName {
            get => _userName;
            set {
                if (_userName != value) {
                    _userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        public string UserEmail {
            get => _userEmail;
            set {
                if (_userEmail != value) {
                    _userEmail = value;
                    OnPropertyChanged(nameof(UserEmail));
                }
            }
        }

        public string ProfileImage {
            get => _profileImage;
            set {
                if (_profileImage != value) {
                    _profileImage = value;
                    OnPropertyChanged(nameof(ProfileImage));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
