namespace EtAlii.xMvvm
{
    /// <summary>
    /// The abstract class of which all different Bindings should inherit.
    /// </summary>
    public abstract class Binding
    {
        public abstract string Id { get; }
        public string Type { get; set; }
    }
}