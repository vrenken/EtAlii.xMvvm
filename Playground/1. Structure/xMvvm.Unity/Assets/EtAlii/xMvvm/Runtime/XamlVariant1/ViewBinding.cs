namespace EtAlii.xMvvm.XamlVariant1
{
    using System.ComponentModel;

    public abstract class ViewBinding<TViewModel> : INotifyPropertyChanged
        where TViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected View<TViewModel> View { get => _view; set => PropertyChanged.SetAndRaise(this, ref _view, value); }
        private View<TViewModel> _view;
        private View<TViewModel> _observedView;

        protected INotifyPropertyChanged ViewModel { get; set; }

        protected ViewBinding(View<TViewModel> view)
            : this()
        {
            View = view;
        }

        protected ViewBinding()
        {
            PropertyChanged += OnPropertyChanged;
        }

        protected abstract void StartBinding();
        protected abstract void StopBinding();
        
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(View):
                    if (_observedView != null) _observedView.PropertyChanged -= OnViewPropertyChanged;
                    _observedView = _view;
                    if (_observedView != null) _observedView.PropertyChanged += OnViewPropertyChanged;
                    break;
            }
        }

        private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(View.ViewModel):
                    if (ViewModel != null)
                    {
                        StopBinding();
                    }

                    ViewModel = View.ViewModel;

                    if (ViewModel != null)
                    {
                        StartBinding();
                    }
                    break;
            }
        }
    }
}