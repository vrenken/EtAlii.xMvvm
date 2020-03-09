namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(Name),
        nameof(Mode),
        nameof(Converter)
    })]
    public class Bind
    {
        public string Name { get; set; }
        
        public StaticResource Converter { get; set; }

        public BindingMode Mode { get; set; } = BindingMode.OneWay;
        
        public object ProvideValue()
        {
            return this;
        }

        public Bind(string name)
        {
            Name = name;
        }
    }
}