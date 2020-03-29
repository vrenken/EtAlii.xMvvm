namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using UnityEngine;

    public class PropertyBinding<TComponent, TViewModel> : ComponentBinding<TComponent, TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        private readonly BindingMode _bindingMode;

        private readonly ComponentListener<TViewModel> _componentListener;
        private readonly ComponentUpdater<TViewModel> _componentUpdater;
        private readonly ViewModelUpdater<TViewModel> _viewModelUpdater;
        private readonly ViewModelListener<TViewModel> _viewModelListener;
        
        public PropertyBinding(
            View<TViewModel> view, 
            string path, 
            Expression<Func<TComponent, object>> component, 
            Expression<Func<TViewModel, object>> vm, 
            BindingMode bindingMode) 
            : base(view, path, component)
        {
            if (vm == null)
            {
                throw new ArgumentNullException(nameof(vm));
            }

            _bindingMode = bindingMode;

            PropertyInfo viewModelPropertyInfo = null;

            switch (vm.Body)
            {
                case MemberExpression memberExpression:
                    viewModelPropertyInfo = memberExpression.Member as PropertyInfo;
                    break;
                case UnaryExpression unaryExpression:
                    viewModelPropertyInfo = (unaryExpression.Operand as MemberExpression)?.Member as PropertyInfo;
                    break;
            }

            if (viewModelPropertyInfo == null)
            {
                throw new InvalidOperationException("Unable to access viewModelProperty from expression: " + vm);
            }

            _viewModelUpdater = new ViewModelUpdater<TViewModel>(view, viewModelPropertyInfo, Component, ComponentMemberExpression);
            _componentUpdater = new ComponentUpdater<TViewModel>(view, viewModelPropertyInfo, Component, ComponentMemberExpression);
            _viewModelListener = new ViewModelListener<TViewModel>(view, viewModelPropertyInfo, _componentUpdater, bindingMode);

            if (_bindingMode != BindingMode.TwoWay && _bindingMode != BindingMode.OneWayToSource) return;
            
            var onValueChangedMember = ComponentType.GetMember("onValueChanged").SingleOrDefault();
            if (onValueChangedMember == null)
            {
                throw new InvalidOperationException("Unable to access onValueChanged from component: " + Component);
            }
            _componentListener = new ComponentListener<TViewModel>(onValueChangedMember, Component, ComponentMemberExpression, _viewModelUpdater);
        }

        protected override void StartBinding()
        {
            if (_bindingMode == BindingMode.OneWayToSource)
            {
                _viewModelUpdater.UpdateFromComponent();
            }
            else
            {
                _componentUpdater.UpdateFromViewModel();
            }

            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWay)
            {
                _viewModelListener.StartListening();
            }

            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWayToSource)
            {
                _componentListener.StartListening();
            }
        }
      
        
        protected override void StopBinding()
        {
            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWayToSource)
            {
                _componentListener.StopListening();
            }

            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWay)
            {
                _viewModelListener.StopListening();
            }
        }
    }
}