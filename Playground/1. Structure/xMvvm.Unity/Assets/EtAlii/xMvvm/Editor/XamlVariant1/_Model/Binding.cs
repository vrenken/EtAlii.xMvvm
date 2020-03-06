namespace EtAlii.xMvvm
{
    public abstract class Binding
    {
        protected Binding(BindingType bindingType)
        {
            BindingType = bindingType;
        }

        protected Binding()
        {
        }

        public BindingType BindingType { get; }
    }
}