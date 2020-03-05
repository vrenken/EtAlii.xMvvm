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

            switch (vm.Body)
            {
                case MemberExpression memberExpression:
                    _viewModelPropertyInfo = memberExpression.Member as PropertyInfo;
                    break;
                case UnaryExpression unaryExpression:
                    _viewModelPropertyInfo = (unaryExpression.Operand as MemberExpression)?.Member as PropertyInfo;
                    break;
                    
            }

            if (_viewModelPropertyInfo == null)
            {
                throw new InvalidOperationException("Unable to access viewModelProperty from expression: " + vm);
            }
        }
        
        protected override void StartBinding() => ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        protected override void StopBinding() => ViewModel.PropertyChanged -= OnViewModelPropertyChanged;

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _viewModelPropertyInfo.Name) return;
            
            UpdateBinding();
        }
        
        protected override void UpdateBinding()
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