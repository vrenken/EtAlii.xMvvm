namespace EtAlii.xMvvm.XamlVariant1
{
    using UnityEngine;
    using System.ComponentModel;

    public class ElementListener<TViewModel>
        where TViewModel: INotifyPropertyChanged
    {
        // ReSharper disable NotAccessedField.Local
        private readonly GameObject _element;
        private readonly ViewModelUpdater<TViewModel> _viewModelUpdater;
        // ReSharper restore NotAccessedField.Local

        public ElementListener(
            GameObject element, 
            ViewModelUpdater<TViewModel> viewModelUpdater)
        {
            _element = element;
            _viewModelUpdater = viewModelUpdater;
        }

        public void StartListening()
        {
            // No way to detect gameobject changes (yet).
        }
        
        public void StopListening()
        {
            // No way to detect gameobject changes (yet).
        }
    }
}