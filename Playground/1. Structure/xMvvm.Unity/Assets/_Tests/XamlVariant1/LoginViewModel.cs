namespace EtAlii.xMvvm
{
    using System.ComponentModel;
    using UnityEngine;

    public class LoginViewModel : INotifyPropertyChanged
    {
        public string UserName { get => _userName; set => PropertyChanged.SetAndRaise(this, ref _userName, value); }
        private string _userName;
        
        public string Password { get => _password; set => PropertyChanged.SetAndRaise(this, ref _password, value); }
        private string _password;

        public bool IsValid { get => _isValid; set => PropertyChanged.SetAndRaise(this, ref _isValid, value); }
        private bool _isValid;

        public event PropertyChangedEventHandler PropertyChanged;

        public LoginViewModel()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsValid = !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);
        }

        public void OnLogin()
        {
            Debug.Log("OnLogin");
        }

        public void OnCancel()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
