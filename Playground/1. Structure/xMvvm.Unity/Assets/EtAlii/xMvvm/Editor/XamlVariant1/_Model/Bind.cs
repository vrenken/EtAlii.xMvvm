namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(Name),
    })]
    public class Bind
    {
        public string Name { get; set; }
        
        public object ProvideValue()
        {
            return this;
        }

        public Bind()
        {
            
        }
        public Bind(string name)
        {
            Name = name;
        }
    }
}