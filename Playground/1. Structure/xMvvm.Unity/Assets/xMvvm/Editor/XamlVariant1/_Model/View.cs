namespace EtAlii.xMvvm
{
    using System.Collections.Generic;

    public class View
    {
        public string ViewModelType { get; set; }
        public string Prefab { get; set; }
        
        [Content]
        public List<Binding> Bindings { get; set; } = new List<Binding>();
    }
}