namespace EtAlii.xMvvm
{
    using System.ComponentModel;
    using EtAlii.xMvvm.XamlVariant1;

    public class NestedViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelTransform SubjectTransformation { get => _subjectTransformation; set => PropertyChanged.SetAndRaise(this, ref _subjectTransformation, value); }
        private ViewModelTransform _subjectTransformation;

        public NestedViewModel()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //IsValid = !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);
        }
    }
}
