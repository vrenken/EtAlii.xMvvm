namespace EtAlii.xMvvm
{
    public static class PascalCaseFilter
    {
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