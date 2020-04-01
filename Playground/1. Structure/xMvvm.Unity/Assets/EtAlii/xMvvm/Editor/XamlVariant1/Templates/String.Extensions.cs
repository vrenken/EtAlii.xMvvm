namespace EtAlii.xMvvm
{
	/// <summary>
	/// A class where we can put simple string extensions.
	/// Remark: These should be simple, and always completely string oriented (i.e. not related to other objects).
	/// </summary>
	public static class StringExtensions 
    {
	    /// <summary>
	    /// Convert the specified string into pascal case.
	    /// </summary>
	    /// <param name="input"></param>
	    /// <returns></returns>
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