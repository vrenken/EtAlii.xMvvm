namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using UnityEngine;

    public abstract class ComponentBinding<TComponent, TViewModel> : ViewBinding<TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        protected MonoBehaviour Component { get; }
        protected MemberInfo ComponentMemberInfo => _componentMemberInfo;
        private readonly MemberInfo _componentMemberInfo;

        protected ComponentBinding(
            View<TViewModel> view,
            string path, 
            Expression<Func<TComponent, object>> component)
        : base(view)
        {
            var child = View.GameObject.transform.Find(path);
            Component = child != null ? child.GetComponent<TComponent>() : null;
            if (Component == null)
            {
                throw new InvalidOperationException($"Unable to find component {typeof(TComponent)} on path {path}");
            }

            MemberHelper.GetMember(component, out var componentMemberExpression);
            _componentMemberInfo = componentMemberExpression.Member;
        }
    }
}