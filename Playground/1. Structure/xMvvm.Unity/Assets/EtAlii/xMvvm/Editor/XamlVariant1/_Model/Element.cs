namespace EtAlii.xMvvm
{
    using System.Collections.Generic;
    using DotLiquid;

    [LiquidType(new []
    {
        nameof(Name),
        nameof(Elements),
        nameof(Path),
        nameof(Transformation),
        nameof(Resources)
    })]
    public class Element
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public ResourceDictionary Resources { get; set; }

        public Bind Transformation { get; set; } // UnityEngine.Transform
        
        public List<Element> Elements { get; } = new List<Element>();
    }
}