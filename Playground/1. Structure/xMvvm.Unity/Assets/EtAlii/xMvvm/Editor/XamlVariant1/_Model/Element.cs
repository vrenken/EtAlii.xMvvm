namespace EtAlii.xMvvm
{
    using System.Collections.Generic;

    public class Element : CodeEntity
    {
        public string Name { get => Id; set => Id = value; }
        public string Path { get; set; }
        public ResourceDictionary Resources { get; set; } = new ResourceDictionary();

        public Bind Transformation { get; set; } // UnityEngine.Transform
        
        public List<Element> Elements { get; } = new List<Element>();
    }
}