namespace EtAlii.xMvvm
{
    using System.Collections.Generic;

    /// <summary>
    /// A base class to proved some code creation helpers. 
    /// </summary>
    public abstract class CodeEntity
    {
        private static readonly Dictionary<string, int> VariableNumbers = new Dictionary<string, int>();
        private static readonly Dictionary<CodeEntity, string> VariableNames = new Dictionary<CodeEntity, string>();
        
        public string Id { get; protected set; }
        protected bool IsField => Id != null;

        /// <summary>
        /// Returns true if a class field needs to be created for this entity. 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool RequiresField(CodeEntity entity) => entity.IsField;
        
        /// <summary>
        /// Returns true if a variable needs to be created for this entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool RequiresVariable(CodeEntity entity) => !entity.IsField;

        /// <summary>
        /// Gets the local name of the entity. Which requires taking into account fields and unique variable numbering.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
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

        /// <summary>
        /// This method needs to be called at the start of each T4 template processing to make sure variable numbering
        /// starts from 1.
        /// </summary>
        public static void ResetVariableNumbering()
        {
            VariableNumbers.Clear();
            VariableNames.Clear();
        }
    }
}