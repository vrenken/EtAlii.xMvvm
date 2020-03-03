namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using UnityEngine;

    public class Binding<TComponent, TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        private readonly string _path;

        public Binding(string path, Func<TComponent, object> componentProperty, Func<TViewModel, object> viewModelProperty)
        {
            _path = path;
        }
    }
}