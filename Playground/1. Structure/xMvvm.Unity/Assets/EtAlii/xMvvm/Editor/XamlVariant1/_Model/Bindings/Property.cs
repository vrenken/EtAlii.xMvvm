namespace EtAlii.xMvvm
{
    /// <summary>
    /// A Binding from ViewModel properties to Component properties or fields. 
    /// </summary>
    public class Property : ComponentBinding
    {
        public Bind Value { get; set; }
    }
}