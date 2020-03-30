namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using UnityEngine;

    public class EventBinding<TComponent, TViewModel> : ComponentBinding<TComponent, TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        private readonly MethodInfo _viewModelMethodInfo;

        public EventBinding(
            View<TViewModel> view, 
            string path, 
            Expression<Func<TComponent, object>> component, 
            Expression<Func<TViewModel, Action>> vm) 
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

        protected override void StartBinding() => EventHelper.AddListener(ComponentMemberExpression.Member, Component, ViewModel, _viewModelMethodInfo);
        protected override void StopBinding() => EventHelper.RemoveListener(ComponentMemberExpression.Member, Component, ViewModel, _viewModelMethodInfo);
    }
}