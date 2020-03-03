namespace EtAlii.xMvvm
{
    public abstract class Element
    {
        protected Element(ElementType elementType)
        {
            ElementType = elementType;
        }

        protected Element()
        {
        }

        public ElementType ElementType { get; }
    }
}