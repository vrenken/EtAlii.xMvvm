// 
// TextTransformation.cs
//  
// Author:
//       Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace Mono.TextTemplating
{
	public abstract class TextTransformation : IDisposable
	{
		private Stack<int> _indents;
		private string _currentIndent = string.Empty;
		private CompilerErrorCollection _errors;
		private StringBuilder _builder;
		private bool _endsWithNewline;

		public virtual void Initialize()
		{
		}

		public abstract string TransformText();

		public virtual IDictionary<string, object> Session { get; set; }

		#region Errors

		public void Error(string message)
		{
			Errors.Add(new CompilerError("", 0, 0, "", message));
		}

		public void Warning(string message)
		{
			Errors.Add(new CompilerError("", 0, 0, "", message) { IsWarning = true });
		}

		protected internal CompilerErrorCollection Errors
		{
			get
			{
				if (_errors == null)
					_errors = new CompilerErrorCollection();
				return _errors;
			}
		}

		private Stack<int> Indents
		{
			get
			{
				if (_indents == null)
					_indents = new Stack<int>();
				return _indents;
			}
		}

		#endregion

		#region Indents

		public string PopIndent()
		{
			if (Indents.Count == 0)
				return "";
			var lastPos = _currentIndent.Length - Indents.Pop();
			var last = _currentIndent.Substring(lastPos);
			_currentIndent = _currentIndent.Substring(0, lastPos);
			return last;
		}

		public void PushIndent(string indent)
		{
			if (indent == null)
				throw new ArgumentNullException("indent");
			Indents.Push(indent.Length);
			_currentIndent += indent;
		}

		public void ClearIndent()
		{
			_currentIndent = string.Empty;
			Indents.Clear();
		}

		public string CurrentIndent
		{
			get { return _currentIndent; }
		}

		#endregion

		#region Writing

		protected StringBuilder GenerationEnvironment
		{
			get
			{
				if (_builder == null)
					_builder = new StringBuilder();
				return _builder;
			}
			set { _builder = value; }
		}

		public void Write(string textToAppend)
		{
			if (string.IsNullOrEmpty(textToAppend))
				return;

			if ((GenerationEnvironment.Length == 0 || _endsWithNewline) && CurrentIndent.Length > 0)
			{
				GenerationEnvironment.Append(CurrentIndent);
			}
			_endsWithNewline = false;

			var last = textToAppend[textToAppend.Length - 1];
			if (last == '\n' || last == '\r')
			{
				_endsWithNewline = true;
			}

			if (CurrentIndent.Length == 0)
			{
				GenerationEnvironment.Append(textToAppend);
				return;
			}

			//insert CurrentIndent after every newline (\n, \r, \r\n)
			//but if there's one at the end of the string, ignore it, it'll be handled next time thanks to endsWithNewline
			var lastNewline = 0;
			for (var i = 0; i < textToAppend.Length - 1; i++)
			{
				var c = textToAppend[i];
				if (c == '\r')
				{
					if (textToAppend[i + 1] == '\n')
					{
						i++;
						if (i == textToAppend.Length - 1)
							break;
					}
				}
				else if (c != '\n')
				{
					continue;
				}
				i++;
				var len = i - lastNewline;
				if (len > 0)
				{
					GenerationEnvironment.Append(textToAppend, lastNewline, i - lastNewline);
				}
				GenerationEnvironment.Append(CurrentIndent);
				lastNewline = i;
			}
			if (lastNewline > 0)
				GenerationEnvironment.Append(textToAppend, lastNewline, textToAppend.Length - lastNewline);
			else
				GenerationEnvironment.Append(textToAppend);
		}

		public void Write(string format, params object[] args)
		{
			Write(string.Format(format, args));
		}

		public void WriteLine(string textToAppend)
		{
			Write(textToAppend);
			GenerationEnvironment.AppendLine();
			_endsWithNewline = true;
		}

		public void WriteLine(string format, params object[] args)
		{
			WriteLine(string.Format(format, args));
		}

		#endregion

		#region Dispose

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		~TextTransformation()
		{
			Dispose(false);
		}

		#endregion
	}
}
