namespace EtAlii.xMvvm
{
    using System.Collections.Generic;

    public abstract class ElementBase : CodeEntity
    {
        public string Name { get => Id; set => Id = value; }
        public string Path { get; set; }

        public ResourceDictionary Resources { get; set; } = new ResourceDictionary();
        
        public List<ElementBase> Elements { get; } = new List<ElementBase>();
    }
}