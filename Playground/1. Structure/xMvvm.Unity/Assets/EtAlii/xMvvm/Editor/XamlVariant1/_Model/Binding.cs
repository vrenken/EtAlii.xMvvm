namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(Name),
        nameof(Source),
        nameof(Target),
        nameof(Mode)
    })]
    public class Binding
    {
        public string Name { get; set; }
        public Property Source { get; set; }
        public Property Target { get; set; }
        public string Mode { get; set; }
    }
}