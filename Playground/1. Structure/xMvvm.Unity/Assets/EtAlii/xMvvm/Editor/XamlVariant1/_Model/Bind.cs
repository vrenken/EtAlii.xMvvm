namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(Name),
        nameof(Mode),
    })]
    public class Bind
    {
        public string Name { get; set; }

        public BindingMode Mode { get; set; } = BindingMode.OneWay;
        
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