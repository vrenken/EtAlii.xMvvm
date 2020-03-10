namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(Key),
        nameof(Type),
    })]
    public abstract class Resource
    {
        public string Type { get; }

        public string Key { get; set; }
        
        protected Resource(string type)
        {
            Type = type;
        }
    }
}