namespace EtAlii.xMvvm.XamlVariant1
{
    using System.ComponentModel;
    using UnityEngine;

    public abstract class View<TViewModel> : INotifyPropertyChanged
        where TViewModel : INotifyPropertyChanged
    {
        public GameObject GameObject { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The ViewModel that should be used to power the view.     
        /// </summary>
        public TViewModel ViewModel { get => _viewModel; set => PropertyChanged.SetAndRaise(this, ref _viewModel, value); }
        private TViewModel _viewModel;

        protected View(GameObject gameObject)
        {
            // Setup a prefab instance. 
            GameObject = gameObject;

        }
    }
}