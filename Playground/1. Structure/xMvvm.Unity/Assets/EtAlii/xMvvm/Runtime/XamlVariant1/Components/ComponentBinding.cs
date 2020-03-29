namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using UnityEngine;

    public abstract class ComponentBinding<TComponent, TViewModel> : ViewBinding<TViewModel>
        where TComponent: MonoBehaviour
        where TViewModel: INotifyPropertyChanged
    {
        protected MonoBehaviour Component { get; }
        protected MemberExpression ComponentMemberExpression { get; }
        protected Type ComponentType { get; }

        protected ComponentBinding(
            View<TViewModel> view,
            string path, 
            Expression<Func<TComponent, object>> component)
        : base(view)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            var child = View.GameObject.transform.Find(path);
            Component = child != null ? child.GetComponent<TComponent>() : null;
            if (Component == null)
            {
                throw new InvalidOperationException($"Unable to find component {typeof(TComponent)} on path {path}");
            }
            ComponentType = Component.GetType();

            switch (component.Body)
            {
                case MemberExpression memberExpression:
                    ComponentMemberExpression  = memberExpression;
                    break;
                case UnaryExpression unaryExpression:
                    ComponentMemberExpression  = unaryExpression.Operand as MemberExpression;
                    break;
            }
            if (ComponentMemberExpression  == null)
            {
                throw new InvalidOperationException("Unable to access component member from expression: " + component);
            }
        }
    }
}