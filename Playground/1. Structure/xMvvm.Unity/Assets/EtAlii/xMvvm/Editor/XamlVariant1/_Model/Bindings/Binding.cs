namespace EtAlii.xMvvm
{
    /// <summary>
    /// The abstract class of which all different Bindings should inherit.
    /// </summary>
    public abstract class Binding : CodeEntity
    {
        public string Type { get; set; }
    }
}