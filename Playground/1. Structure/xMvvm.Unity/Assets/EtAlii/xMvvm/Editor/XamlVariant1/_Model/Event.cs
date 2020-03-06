namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(Name),
        nameof(Path),
        nameof(Member),
        nameof(Type),
        nameof(BindingType),

        nameof(Handler),
    })]
    public class Event : ComponentBinding
    {
        public Bind Handler { get; set; }
        
        public Event()
            : base(BindingType.Event)
        {
        }
    }
}