namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using UnityEngine;

    public class ElementBinding
    {
        public GameObject GameObject { get; private set; }
        public ElementBinding[] Elements { get; }

        public string Path { get; }

        public ElementBinding(GameObject gameObject)
        {
            // Setup a prefab instance. 
            GameObject = gameObject;

            Path = String.Empty;
            Elements = Array.Empty<ElementBinding>();
        }

        public ElementBinding(string path)
            : this(path, Array.Empty<ElementBinding>())
        {
        }

        public ElementBinding(string path, ElementBinding[] elements)
        {
            Path = path;
            Elements = elements;
        }

        public ElementBinding(GameObject parentGameObject, string path)
            : this(parentGameObject, path, Array.Empty<ElementBinding>())
        {
        }

        public ElementBinding(GameObject parentGameObject, string path, ElementBinding[] elements)
        {
            Path = path;
            Elements = elements;

            DelayedInitialize(parentGameObject);
        }

        private void DelayedInitialize(GameObject parentGameObject)
        {
            GameObject = parentGameObject.transform.Find(Path).gameObject;
            //GameObject.transform.parent = parent.GameObject.transform;
            
            foreach (var element in Elements)
            {
                element.DelayedInitialize(GameObject);                
            }
        }
    }
}