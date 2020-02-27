namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(Name)
    })]
    public class Binding
    {
        public string Name { get; set; }
    }
}