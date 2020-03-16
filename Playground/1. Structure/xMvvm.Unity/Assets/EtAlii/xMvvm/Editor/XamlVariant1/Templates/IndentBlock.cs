namespace EtAlii.xMvvm
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using DotLiquid;

    public class IndentBlock : Block
    {
        private static int _indents = 3;

        private int _currentIndent;
        private bool _skipFirstLine;

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            _currentIndent = _indents;
            _indents += 1;
            base.Initialize(tagName, markup, tokens);
            
            _skipFirstLine = Convert.ToBoolean(markup);
        }
	
        public override void Render(Context context, TextWriter result)
        {
            using (var stream = new MemoryStream())
            {
                using (var subResult = new StreamWriter(stream))
                {
                    base.Render(context, subResult);

                    subResult.Flush();
                    stream.Position = 0;
                    
                    using (var reader = new StreamReader(stream))
                    {
                        var content = reader.ReadToEnd();

                        var indentText = string.Concat(Enumerable.Repeat("    ", _currentIndent));

                        var lines = content.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
                        for (var i = 0; i < lines.Length; i++)
                        {
                            lines[i] = i == 0 && _skipFirstLine 
                                ? lines[i]
                                : indentText + lines[i];
                        }

                        content = string.Join(Environment.NewLine, lines);
                    
                        result.Write(content);
                    }
                }
            }
            
            _indents -= 1;
        }
    }
}