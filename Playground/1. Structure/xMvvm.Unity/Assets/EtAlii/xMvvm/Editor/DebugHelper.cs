namespace EtAlii.xMvvm
{
    using System;

    public static class DebugHelper
    {
        public static string AddLineNumbers(string text)
        {
            // We want to have a padded line number
            // in front of each line.

            // So let's fetch all individual XAML lines.
            var lines = text.Split(new []{ Environment.NewLine}, StringSplitOptions.None);

            // Determine the padding count.
            // (probably no XAML files with more than 1 million lines...)
            var count = lines.Length;
            var spacing =
                count < 10 ? 1 :
                count < 100 ? 2 :
                count < 1000 ? 3 :
                count < 10000 ? 4 : 5;
                
            // ... Prefix each line.
            var format = $"D{spacing}";
            for (var i = 0; i < count; i++)
            {
                lines[i] = $"{i.ToString(format)}: {lines[i]}";
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
