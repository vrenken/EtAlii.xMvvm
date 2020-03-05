namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Events;

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

            var unaryExpression = vm.Body as UnaryExpression;
            var methodCallExpression = unaryExpression?.Operand as  MethodCallExpression;
            var typedConstantExpression = methodCallExpression?.Object as ConstantExpression;
            _viewModelMethodInfo = typedConstantExpression?.Value as MethodInfo;
            if (_viewModelMethodInfo == null)
            {
                throw new InvalidOperationException("Unable to access viewModelMethod from expression: " + vm);
            }
        }

        protected override void StartBinding()
        {
            var handler = _viewModelMethodInfo;
            var listener = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), ViewModel, handler);

            switch (ComponentMemberExpression.Member)
            {
                case PropertyInfo componentPropertyInfo:
                    var unityPropertyEvent = (UnityEvent)componentPropertyInfo.GetValue(Component);
                    unityPropertyEvent.AddListener(listener);
                    break;
                case FieldInfo componentFieldInfo: 
                    var unityFieldEvent = (UnityEvent)componentFieldInfo.GetValue(Component);
                    unityFieldEvent.AddListener(listener);
                    break;
            }
        }

        protected override void StopBinding()
        {
            var handler = _viewModelMethodInfo;
            var listener = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), ViewModel, handler);

            switch (ComponentMemberExpression.Member)
            {
                case PropertyInfo componentPropertyInfo:
                    var unityPropertyEvent = (UnityEvent)componentPropertyInfo.GetValue(Component);
                    unityPropertyEvent.RemoveListener(listener);
                    break;
                case FieldInfo componentFieldInfo: 
                    var unityFieldEvent = (UnityEvent)componentFieldInfo.GetValue(Component);
                    unityFieldEvent.RemoveListener(listener);
                    break;
            }
        }
    }
}