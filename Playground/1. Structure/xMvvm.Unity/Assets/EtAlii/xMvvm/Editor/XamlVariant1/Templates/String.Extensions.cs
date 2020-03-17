namespace EtAlii.xMvvm
{
	public static class StringExtensions 
    {
	    public static string PascalCase(this string input)
	    {
		    if (string.IsNullOrEmpty(input))
		    {
			    return string.Empty;
		    }

		    var characters = input.ToCharArray();
		    characters[0] = char.ToLower(characters[0]);

		    return new string(characters);
	    }
    }
}