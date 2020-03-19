namespace EtAlii.xMvvm
{
    using System.Collections.Generic;

    public class CodeEntity
    {
        private static readonly Dictionary<string, int> VariableNumbers = new Dictionary<string, int>();
        private static readonly Dictionary<CodeEntity, string> VariableNames = new Dictionary<CodeEntity, string>();
        
        public string Id { get; protected set; }
        protected bool IsField => Id != null;

        public static bool RequiresField(CodeEntity entity) => entity.IsField;
        public static bool RequiresVariable(CodeEntity entity) => !entity.IsField;

        public static string GetLocalName(CodeEntity entity)
        {
            if (entity.IsField)
            {
                return "_" + entity.Id.PascalCase();
            }

            if (VariableNames.TryGetValue(entity, out var existingName))
            {
                return existingName;
            }
                
            var name = entity.GetType().Name.PascalCase();

            if (VariableNumbers.ContainsKey(name) == false)
            {
                VariableNumbers[name] = 0;
            }

            var number = VariableNumbers[name] += 1;

            var newName = name + number;

            VariableNames[entity] = newName;

            return newName;
        }

        public static void ResetVariableNumbering()
        {
            VariableNumbers.Clear();
            VariableNames.Clear();
        }
    }
}