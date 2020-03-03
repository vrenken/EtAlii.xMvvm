namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using UnityEngine;

    public class PropertyBinding<TComponent, TViewModel> : Binding<TComponent, TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        public PropertyBinding(string path, Func<TComponent, object> componentProperty, Func<TViewModel, object> viewModelProperty) 
            : base(path, componentProperty, viewModelProperty)
        {
        }
    }
}