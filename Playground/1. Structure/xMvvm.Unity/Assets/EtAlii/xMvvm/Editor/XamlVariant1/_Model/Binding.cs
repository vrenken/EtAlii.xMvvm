namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(Name),
        nameof(Component),
        nameof(ComponentProperty),
        nameof(ViewModelProperty),
        nameof(Mode)
    })]
    public class Binding
    {
        public string Name { get; set; }
        public string Component { get; set; }
        public string ComponentProperty { get; set; }
        public string ViewModelProperty { get; set; }
        public string Mode { get; set; }
    }
}