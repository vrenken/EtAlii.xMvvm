namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using UnityEngine;

    public class PropertyBinding<TComponent, TViewModel> : Binding<TComponent, TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        private readonly PropertyInfo _viewModelPropertyInfo;

        public PropertyBinding(View<TViewModel> view, string path, Expression<Func<TComponent, object>> component, Expression<Func<TViewModel, object>> vm) 
            : base(view, path, component)
        {
            if (vm == null)
            {
                throw new ArgumentNullException(nameof(vm));
            }

            var viewModelMemberExpression = vm.Body as MemberExpression;
            _viewModelPropertyInfo = viewModelMemberExpression?.Member as PropertyInfo;
            if (_viewModelPropertyInfo == null)
            {
                throw new InvalidOperationException("Unable to access viewModelProperty from expression: " + vm);
            }
        }
        
        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _viewModelPropertyInfo.Name) return;
            
            SetComponentPropertyValue();
        }
        
        protected override void SetComponentPropertyValue()
        {
            var value = _viewModelPropertyInfo.GetValue(ViewModel);
                
            switch (ComponentMemberExpression.Member)
            {
                case PropertyInfo componentPropertyInfo: 
                    componentPropertyInfo.SetValue(Component, value, null);
                    break;
                case FieldInfo componentFieldInfo: 
                    componentFieldInfo.SetValue(Component, value);
                    break;
            }
        }

    }
}