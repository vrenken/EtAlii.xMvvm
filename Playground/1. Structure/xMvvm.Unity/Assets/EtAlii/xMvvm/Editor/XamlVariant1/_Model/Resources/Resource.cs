namespace EtAlii.xMvvm
{
    public abstract class Resource : CodeEntity
    {
        public string Type { get; }

        public string Key { get => Id; set => Id = value; }
        
        protected Resource(string type)
        {
            Type = type;
        }
    }
}