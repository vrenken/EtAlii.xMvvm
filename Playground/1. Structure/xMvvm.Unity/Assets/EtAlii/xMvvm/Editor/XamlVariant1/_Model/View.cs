namespace EtAlii.xMvvm
{
    using System.Collections.Generic;

    public class View : Element
    {
        public string ViewModelType { get; set; }
        public string Prefab { get; set; }
        
        public List<Binding> Bindings { get; } = new List<Binding>();
    }
}