namespace EtAlii.xMvvm
{
    using System;
    using System.IO;

    public class Variant2ViewCodeManager : IViewCodeManager
    {
        private const string XamlFileExtension = ".v2xaml";

        public bool CanManage(string asset) => string.Compare(Path.GetExtension(asset), XamlFileExtension, StringComparison.OrdinalIgnoreCase) == 0;

        public void Delete(string asset) 
        {
        }

        public void Create(string asset)
        {
        }
    }
}
