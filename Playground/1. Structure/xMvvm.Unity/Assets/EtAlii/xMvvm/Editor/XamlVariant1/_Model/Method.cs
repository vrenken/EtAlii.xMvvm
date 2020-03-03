namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(Name)
    })]
    public class Method : Element
    {
        [Content]
        public string Name { get; set; }
    }
}