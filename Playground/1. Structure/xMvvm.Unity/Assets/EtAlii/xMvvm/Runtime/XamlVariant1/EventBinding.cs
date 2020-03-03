namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using UnityEngine;

    public class EventBinding<TComponent, TViewModel> : Binding<TComponent, TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        public EventBinding(string path, Func<TComponent, object> componentEvent, Func<TViewModel, object> viewModelMethod) 
            : base(path, componentEvent, viewModelMethod)
        {
        }
    }
}