namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using UnityEngine;

    public class EventBinding<TComponent, TViewModel> : Binding<TComponent, TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        public EventBinding(View<TViewModel> view, string path, Expression<Func<TComponent, object>> component, Expression<Func<TViewModel, object>> vm) 
            : base(view, path, component, vm)
        {
        }
    }
}