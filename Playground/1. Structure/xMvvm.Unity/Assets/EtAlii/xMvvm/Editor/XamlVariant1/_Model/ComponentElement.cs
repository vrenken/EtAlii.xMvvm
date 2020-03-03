namespace EtAlii.xMvvm
{
    public abstract class ComponentElement : Element
    {
        [Content]
        public string Name { get; set; }
        
        public string Path { get; set; }
        public string Member { get; set; }
        public string Type { get; set; }
        
        protected ComponentElement(ElementType elementType)
            : base(elementType)
        {
        }
    }
}