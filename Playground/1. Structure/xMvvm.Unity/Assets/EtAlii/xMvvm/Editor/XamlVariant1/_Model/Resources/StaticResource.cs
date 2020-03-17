namespace EtAlii.xMvvm
{
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