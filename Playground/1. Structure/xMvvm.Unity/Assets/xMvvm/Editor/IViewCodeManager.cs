namespace EtAlii.xMvvm
{
    public interface IViewCodeManager
    {
        bool CanManage(string asset);
        
        void Delete(string asset);
        void Create(string asset);
    }
}
