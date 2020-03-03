namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using UnityEngine;

    public abstract class Binding<TComponent, TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        private readonly string _path;

        protected Binding(string path, Func<TComponent, object> componentProperty, Func<TViewModel, object> viewModelProperty)
        {
            _path = path;
        }
    }
}