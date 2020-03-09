namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(Key)
    })]
    public class Resource
    {
        public string Key { get; set; }
    }
}