namespace EtAlii.xMvvm
{
    public class CodeEntity
    {
        public string Id { get; protected set; }
        protected bool IsField { get; set; } = true;

        public static bool RequiresField(CodeEntity entity) => entity.IsField;

        public static string GetLocalName(CodeEntity entity)
        {
            return (entity.IsField ? "_" : "") + entity.Id.PascalCase();
        }
    }
}