namespace EtAlii.xMvvm
{
    /// <summary>
    /// A Binding that targets Components. 
    /// </summary>
    public abstract class ComponentBinding : Binding
    {
        [Content]
        public string Name { get => Id; set => Id = value; }
        
        public string Path { get; set; }
        public string Member { get; set; }
    }
}