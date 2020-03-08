namespace EtAlii.xMvvm.XamlVariant1
{
    using UnityEngine;

    public class Element
    {
        public GameObject GameObject { get; }

        public Element(GameObject gameObject)
        {
            // Setup a prefab instance. 
            GameObject = gameObject;
        }

        public Element(GameObject gameObject, Element parent, string name)
            : this(gameObject)
        {
            GameObject.transform.parent = parent.GameObject.transform;
        }
    }
}