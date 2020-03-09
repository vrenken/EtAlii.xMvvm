namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(ResourceKey),
    })]
    public class StaticResource
    {
        [Content]
        public string ResourceKey { get; set; }

        public StaticResource(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public StaticResource ProvideValue()
        {
            return this;
        }
    }
}