namespace EtAlii.xMvvm
{
    /// <summary>
    /// The abstract class of which all different Bindings should inherit.
    /// </summary>
    public abstract class Binding
    {
        protected Binding(BindingType bindingType)
        {
            BindingType = bindingType;
        }

        public BindingType BindingType { get; }
    }
}