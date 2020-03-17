namespace EtAlii.xMvvm
{
    using System.Collections.Generic;

    public class ResourceDictionary 
    {
        // [Content, DeferredContent]
        // public object DeferredContent { get; set; }
        
        [Content]
        public List<Resource> Items { get; } = new List<Resource>();
    }
}