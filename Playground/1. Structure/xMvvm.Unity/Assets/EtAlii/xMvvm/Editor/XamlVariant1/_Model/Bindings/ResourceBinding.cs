namespace EtAlii.xMvvm
{
    /// <summary>
    /// A Binding that targets Components. 
    /// </summary>
    public abstract class ResourceBinding : Binding
    {
        [Content]
        public string ResourceKey { get => Id; set => Id = value; }
    }
}