namespace EtAlii.xMvvm
{
    using DotLiquid;

    [LiquidType( new []
    {
        nameof(Name),
        nameof(Path),
        nameof(Member),
        nameof(Type),
        nameof(ElementType),

        nameof(Handler),
    })]
    public class Event : ComponentElement
    {
        public Bind Handler { get; set; }
        
        public Event()
            : base(ElementType.Event)
        {
        }
    }
}