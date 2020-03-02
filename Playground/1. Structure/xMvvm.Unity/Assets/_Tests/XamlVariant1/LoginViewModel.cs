namespace EtAlii.xMvvm
{
    using System.ComponentModel;

    public class LoginViewModel : INotifyPropertyChanged
    {
        public string UserName { get => _userName; set => PropertyChanged.SetAndRaise(this, ref _userName, value); }
        private string _userName;
        
        public string Password { get => _password; set => PropertyChanged.SetAndRaise(this, ref _password, value); }
        private string _password;
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
