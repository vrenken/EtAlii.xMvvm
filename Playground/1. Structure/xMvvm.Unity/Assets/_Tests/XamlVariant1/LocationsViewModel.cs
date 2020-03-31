namespace EtAlii.xMvvm
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class LocationsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<object> Locations => _locations;
        private readonly ObservableCollection<object> _locations = new ObservableCollection<object>();

        public LocationsViewModel()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //IsValid = !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);
        }
    }
}
