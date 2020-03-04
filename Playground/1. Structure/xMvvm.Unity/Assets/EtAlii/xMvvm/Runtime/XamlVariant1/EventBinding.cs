namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using UnityEngine;

    public class EventBinding<TComponent, TViewModel> : Binding<TComponent, TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        private readonly MethodInfo _viewModelMethodInfo;

        public EventBinding(View<TViewModel> view, string path, Expression<Func<TComponent, object>> component, Expression<Func<TViewModel, Action>> vm) 
            : base(view, path, component)
        {
            if (vm == null)
            {
                throw new ArgumentNullException(nameof(vm));
            }

            var viewModelMemberExpression = vm.Body as MemberExpression;
            _viewModelMethodInfo = viewModelMemberExpression?.Member as MethodInfo;
            if (_viewModelMethodInfo == null)
            {
                throw new InvalidOperationException("Unable to access viewModelMethod from expression: " + vm);
            }
        }
        
        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _viewModelMethodInfo.Name) return;
            
            SetComponentPropertyValue();
        }
        
        protected override void SetComponentPropertyValue()
        {
            // var value = _viewModelMethodInfo.GetValue(ViewModel);
            //     
            // switch (ComponentMemberExpression.Member)
            // {
            //     case MethodInfo componentMethodInfo: 
            //         componentPropertyInfo.SetValue(Component, value, null);
            //         break;
            //     case FieldInfo componentFieldInfo: 
            //         componentFieldInfo.SetValue(Component, value);
            //         break;
            // }
        }

    }
}