namespace EtAlii.xMvvm
{
    /// <summary>
    /// A Binding that targets Components. 
    /// </summary>
    public abstract class ComponentBinding : Binding
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