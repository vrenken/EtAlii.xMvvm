namespace EtAlii.xMvvm
{
    using System.Collections.Generic;
    using DotLiquid;

    [LiquidType(new []
    {
        nameof(Items)        
    })]
    public class ResourceDictionary 
    {
        // [Content, DeferredContent]
        // public object DeferredContent { get; set; }
        
        [Content]
        public List<Resource> Items  { get; } = new List<Resource>();
    }
}