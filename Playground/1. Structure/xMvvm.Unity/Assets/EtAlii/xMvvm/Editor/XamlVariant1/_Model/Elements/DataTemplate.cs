namespace EtAlii.xMvvm
{
    using System.Collections.Generic;

    public class DataTemplate 
    {
        public string Prefab { get; set; }
        public List<Binding> Bindings { get; } = new List<Binding>();
    }
}