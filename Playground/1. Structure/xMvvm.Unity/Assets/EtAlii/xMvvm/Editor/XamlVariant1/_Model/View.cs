namespace EtAlii.xMvvm
{
    using System.Collections.Generic;
    using DotLiquid;

    [LiquidType(new []
    {
        nameof(ViewModelType),
        nameof(Prefab),
        nameof(Elements)
    })]
    public class View
    {
        public string ViewModelType { get; set; }
        public string Prefab { get; set; }
        
        [Content]
        public List<Element> Elements { get; set; } = new List<Element>();
    }
}