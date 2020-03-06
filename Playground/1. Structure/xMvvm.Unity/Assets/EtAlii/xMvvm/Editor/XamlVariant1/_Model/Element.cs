namespace EtAlii.xMvvm
{
    public abstract class Element
    {
        protected Element(BindingType bindingType)
        {
            BindingType = bindingType;
        }

        protected Element()
        {
        }

        public BindingType BindingType { get; }
    }
}