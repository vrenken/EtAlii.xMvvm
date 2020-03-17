namespace EtAlii.xMvvm
{
    public abstract class Resource
    {
        public string Type { get; }

        public string Key { get; set; }
        
        protected Resource(string type)
        {
            Type = type;
        }
    }
}