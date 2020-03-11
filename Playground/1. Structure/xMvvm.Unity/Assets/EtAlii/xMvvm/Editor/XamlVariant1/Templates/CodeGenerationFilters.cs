namespace EtAlii.xMvvm
{
    using System;

    public static class CodeGenerationFilters
    {
        public static string Indent(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            
            var lines = input.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            for (var i = 0; i < lines.Length; i++)
            {
                lines[i] = "    " + lines[i]; 
            }

            return string.Join(Environment.NewLine, lines);
        }

        public static string PascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var characters = input.ToCharArray();
            characters[0] = char.ToLower(characters[0]);

            return new string(characters);
        }
        
        public static string PrivateMember(string input) => "_" + PascalCase(input);
    }
}