namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Events;

    public class PropertyBinding<TComponent, TViewModel> : Binding<TComponent, TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        private readonly BindingMode _bindingMode;
        private readonly PropertyInfo _viewModelPropertyInfo;
        private readonly MemberInfo _onValueChangedMember;

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

            if (_bindingMode != BindingMode.TwoWay && _bindingMode != BindingMode.OneWayToSource) return;
            
            _onValueChangedMember = ComponentType.GetMember("onValueChanged").SingleOrDefault();
            if (_onValueChangedMember == null)
            {
                throw new InvalidOperationException("Unable to access onValueChanged from component: " + Component);
            }
        }

        protected override void StartBinding()
        {
            if (_bindingMode == BindingMode.OneWayToSource)
            {
                UpdateViewModelFromComponent();
            }
            else
            {
                UpdateComponentFromViewModel();
            }

            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWay)
            {
                ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            }

            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWayToSource)
            {
                StartListeningToComponent();
            }
        }
        
        private void StartListeningToComponent()
        {
            switch (_onValueChangedMember)
            {
                case PropertyInfo propertyInfo:
                    var unityPropertyEvent = (UnityEventBase)propertyInfo.GetValue(Component);
                    AddListener(unityPropertyEvent, UpdateViewModelFromComponent);
                    break;
                case FieldInfo fieldInfo: 
                    var unityFieldEvent = (UnityEventBase)fieldInfo.GetValue(Component);
                    AddListener(unityFieldEvent, UpdateViewModelFromComponent);
                    break;
            }
        }

        private void AddListener(UnityEventBase unityEvent, UnityAction action)
        {
            switch (unityEvent)
            {
                case UnityEvent actionEvent:
                    actionEvent.AddListener(action);
                    break;
                case UnityEvent<string> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<float> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<int> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<DateTime> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<Byte> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<Char> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<uint> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                case UnityEvent<ulong> actionEvent:
                    actionEvent.AddListener(o => action());
                    break;                    
                default:
                    throw new NotSupportedException("UnityEvent not supported: " + unityEvent);
            }
        }
        private void RemoveListener(UnityEventBase unityEvent, UnityAction action)
        {
            switch (unityEvent)
            {
                case UnityEvent actionEvent:
                    actionEvent.RemoveListener(action);
                    break;
                case UnityEvent<string> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                case UnityEvent<float> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                case UnityEvent<int> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                case UnityEvent<DateTime> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                case UnityEvent<Byte> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                case UnityEvent<Char> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                case UnityEvent<uint> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                case UnityEvent<ulong> actionEvent:
                    actionEvent.RemoveListener(o => action());
                    break;                    
                default:
                    throw new NotSupportedException("UnityEvent not supported: " + unityEvent);
            }
        }
        
        protected override void StopBinding()
        {
            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWayToSource)
            {
                StopListeningToComponent();
            }

            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWay)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }
        
        private void StopListeningToComponent()
        {
            switch (ComponentMemberExpression.Member)
            {
                case PropertyInfo propertyInfo:
                    var unityPropertyEvent = (UnityEventBase)propertyInfo.GetValue(Component);
                    RemoveListener(unityPropertyEvent, UpdateViewModelFromComponent);
                    break;
                case FieldInfo fieldInfo: 
                    var unityFieldEvent = (UnityEventBase)fieldInfo.GetValue(Component);
                    RemoveListener(unityFieldEvent, UpdateViewModelFromComponent);
                    break;
            }
        }


        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _viewModelPropertyInfo.Name) return;

            if (_bindingMode != BindingMode.OneWayToSource)
            {
                UpdateComponentFromViewModel();
            }
        }
        
        private void UpdateComponentFromViewModel()
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
        
        private void UpdateViewModelFromComponent()
        {
            object value = null;
            
            switch (ComponentMemberExpression.Member)
            {
                case PropertyInfo componentPropertyInfo: 
                    value = componentPropertyInfo.GetValue(Component, null);
                    break;
                case FieldInfo componentFieldInfo: 
                    value = componentFieldInfo.GetValue(Component);
                    break;
            }
            _viewModelPropertyInfo.SetValue(ViewModel, value);
        }
    }
}