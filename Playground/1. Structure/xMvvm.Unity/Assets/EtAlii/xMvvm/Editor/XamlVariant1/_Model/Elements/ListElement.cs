namespace EtAlii.xMvvm
{
    /// <summary>
    /// This class represents an element that visualizes a list of elements.  
    /// </summary>
    public class ListElement : ElementBase
    {
        /// <summary>
        /// The source of the items that should be visualized. 
        /// </summary>
        public Bind ItemsSource { get; set; }
        
        public object ItemContainer { get; set; }
        public DataTemplate ItemTemplate { get; set; }
    }
}