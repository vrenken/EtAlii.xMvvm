namespace EtAlii.xMvvm
{
    public abstract class ComponentBinding : Element
    {
        [Content]
        public string Name { get; set; }
        
        public string Path { get; set; }
        public string Member { get; set; }
        public string Type { get; set; }
        
        protected ComponentBinding(BindingType bindingType)
            : base(bindingType)
        {
        }
    }
}