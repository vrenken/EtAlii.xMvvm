namespace EtAlii.xMvvm
{
    using System.ComponentModel;

    public class CompositionViewModel : INotifyPropertyChanged
    {
        public float Up { get => _up; set => PropertyChanged.SetAndRaise(this, ref _up, value); }
        private float _up;

        public float Forward { get => _forward; set => PropertyChanged.SetAndRaise(this, ref _forward, value); }
        private float _forward;

        public bool Rotate { get => _rotate; set => PropertyChanged.SetAndRaise(this, ref _rotate, value); }
        private bool _rotate;

        public event PropertyChangedEventHandler PropertyChanged;

        public CompositionViewModel()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

    }
}
