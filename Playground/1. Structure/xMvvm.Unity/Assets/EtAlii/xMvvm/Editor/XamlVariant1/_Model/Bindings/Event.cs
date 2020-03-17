namespace EtAlii.xMvvm
{
    /// <summary>
    /// A Binding from Component (Unity) events to ViewModel methods.  
    /// </summary>
    public class Event : ComponentBinding
    {
        public Bind Handler { get; set; }
        
        public Event()
            : base(BindingType.Event)
        {
        }
    }
}