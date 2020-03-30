namespace EtAlii.xMvvm
{
    using System.ComponentModel;
    using EtAlii.xMvvm.XamlVariant1;

    public class CompositionViewModel : INotifyPropertyChanged
    {
        public float Up { get => _up; set => PropertyChanged.SetAndRaise(this, ref _up, value); }
        private float _up;

        public float Forward { get => _forward; set => PropertyChanged.SetAndRaise(this, ref _forward, value); }
        private float _forward;

        public bool Rotate { get => _rotate; set => PropertyChanged.SetAndRaise(this, ref _rotate, value); }
        private bool _rotate;
        
        // Wrong! THe VM should have no knowledge of the views and therefore also never translate/rotate/scale insights.
        // We keep this in here for testing purposes.
        public ViewModelTransform SubjectTransformation { get => _subjectTransformation; set => PropertyChanged.SetAndRaise(this, ref _subjectTransformation, value); }
        private ViewModelTransform _subjectTransformation = ViewModelTransform.Empty;

        public ViewModelTransform CanvasTransformation { get => _canvasTransformation; set => PropertyChanged.SetAndRaise(this, ref _canvasTransformation, value); }
        private ViewModelTransform _canvasTransformation = ViewModelTransform.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public CompositionViewModel()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Debug.Log($"{nameof(CompositionView)} property changed: {e.PropertyName}");

            switch (e.PropertyName)
            {
                case nameof(Up):
                case nameof(Forward):
                    SubjectTransformation = SubjectTransformation.SetPosition(0, Up, Forward);
                    break;
                // case nameof(Rotate):
                //      SubjectTransformation = SubjectTransformation.SetRotation(0, Rotate, 1f);
                //      break;
            }
        }

    }
}
