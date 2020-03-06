namespace EtAlii.xMvvm
{
    using System.Collections.Generic;
    using DotLiquid;

    [LiquidType(new []
    {
        nameof(ViewModelType),
        nameof(Prefab),
        nameof(Bindings)
    })]
    public class View
    {
        public string ViewModelType { get; set; }
        public string Prefab { get; set; }
        
        public List<Binding> Bindings { get; } = new List<Binding>();
    }
}