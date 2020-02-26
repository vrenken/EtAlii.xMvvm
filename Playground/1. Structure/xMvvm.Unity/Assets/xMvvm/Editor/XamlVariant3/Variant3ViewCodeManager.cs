namespace EtAlii.xMvvm
{
    using System;
    using System.IO;

    public class Variant3ViewCodeManager : IViewCodeManager
    {
        private const string XamlFileExtension = ".v3xaml";

        public bool CanManage(string asset) => string.Compare(Path.GetExtension(asset), XamlFileExtension, StringComparison.OrdinalIgnoreCase) == 0;

        public void Delete(string asset)
        {
        }

        public void Create(string asset)
        {
        }
    }
}
