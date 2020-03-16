// 
// Tokeniser.cs
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

// ReSharper disable all

using System;

namespace Mono.TextTemplating
{
	public class Tokeniser
	{
		private readonly string _content;
		private int _position;
		private string _value;
		private State _nextState = State.Content;
		private Location _nextStateLocation;
		private Location _nextStateTagStartLocation;

		public Tokeniser(string fileName, string content)
		{
			State = State.Content;
			this._content = content;
			Location = _nextStateLocation = _nextStateTagStartLocation = new Location(fileName, 1, 1);
		}

		public bool Advance()
		{
			_value = null;
			State = _nextState;
			Location = _nextStateLocation;
			TagStartLocation = _nextStateTagStartLocation;
			if (_nextState == State.Eof)
				return false;
			_nextState = GetNextStateAndCurrentValue();
			return true;
		}

		private State GetNextStateAndCurrentValue()
		{
			switch (State)
			{
				case State.Block:
				case State.Expression:
				case State.Helper:
					return GetBlockEnd();

				case State.Directive:
					return NextStateInDirective();

				case State.Content:
					return NextStateInContent();

				case State.DirectiveName:
					return GetDirectiveName();

				case State.DirectiveValue:
					return GetDirectiveValue();

				default:
					throw new InvalidOperationException("Unexpected state '" + State + "'");
			}
		}

		private State GetBlockEnd()
		{
			var start = _position;
			for (; _position < _content.Length; _position++)
			{
				var c = _content[_position];
				_nextStateTagStartLocation = _nextStateLocation;
				_nextStateLocation = _nextStateLocation.AddCol();
				if (c == '\r')
				{
					if (_position + 1 < _content.Length && _content[_position + 1] == '\n')
						_position++;
					_nextStateLocation = _nextStateLocation.AddLine();
				}
				else if (c == '\n')
				{
					_nextStateLocation = _nextStateLocation.AddLine();
				}
				else if (c == '>' && _content[_position - 1] == '#' && _content[_position - 2] != '\\')
				{
					_value = _content.Substring(start, _position - start - 1);
					_position++;
					TagEndLocation = _nextStateLocation;

					//skip newlines directly after blocks, unless they're expressions
					if (State != State.Expression && (_position += IsNewLine()) > 0)
					{
						_nextStateLocation = _nextStateLocation.AddLine();
					}
					return State.Content;
				}
			}
			throw new ParserException("Unexpected end of file.", _nextStateLocation);
		}

		private State GetDirectiveName()
		{
			var start = _position;
			for (; _position < _content.Length; _position++)
			{
				var c = _content[_position];
				if (!Char.IsLetterOrDigit(c))
				{
					_value = _content.Substring(start, _position - start);
					return State.Directive;
				}
				_nextStateLocation = _nextStateLocation.AddCol();
			}
			throw new ParserException("Unexpected end of file.", _nextStateLocation);
		}

		private State GetDirectiveValue()
		{
			var start = _position;
			int delimiter = '\0';
			for (; _position < _content.Length; _position++)
			{
				var c = _content[_position];
				_nextStateLocation = _nextStateLocation.AddCol();
				if (c == '\r')
				{
					if (_position + 1 < _content.Length && _content[_position + 1] == '\n')
						_position++;
					_nextStateLocation = _nextStateLocation.AddLine();
				}
				else if (c == '\n')
					_nextStateLocation = _nextStateLocation.AddLine();
				if (delimiter == '\0')
				{
					if (c == '\'' || c == '"')
					{
						start = _position;
						delimiter = c;
					}
					else if (!Char.IsWhiteSpace(c))
					{
						throw new ParserException("Unexpected character '" + c + "'. Expecting attribute value.", _nextStateLocation);
					}
					continue;
				}
				if (c == delimiter)
				{
					_value = _content.Substring(start + 1, _position - start - 1);
					_position++;
					return State.Directive;
				}
			}
			throw new ParserException("Unexpected end of file.", _nextStateLocation);
		}

		private State NextStateInContent()
		{
			var start = _position;
			for (; _position < _content.Length; _position++)
			{
				var c = _content[_position];
				_nextStateTagStartLocation = _nextStateLocation;
				_nextStateLocation = _nextStateLocation.AddCol();
				if (c == '\r')
				{
					if (_position + 1 < _content.Length && _content[_position + 1] == '\n')
						_position++;
					_nextStateLocation = _nextStateLocation.AddLine();
				}
				else if (c == '\n')
				{
					_nextStateLocation = _nextStateLocation.AddLine();
				}
				else if (c == '<' && _position + 2 < _content.Length && _content[_position + 1] == '#')
				{
					TagEndLocation = _nextStateLocation;
					var type = _content[_position + 2];
					if (type == '@')
					{
						_nextStateLocation = _nextStateLocation.AddCols(2);
						_value = _content.Substring(start, _position - start);
						_position += 3;
						return State.Directive;
					}
					if (type == '=')
					{
						_nextStateLocation = _nextStateLocation.AddCols(2);
						_value = _content.Substring(start, _position - start);
						_position += 3;
						return State.Expression;
					}
					if (type == '+')
					{
						_nextStateLocation = _nextStateLocation.AddCols(2);
						_value = _content.Substring(start, _position - start);
						_position += 3;
						return State.Helper;
					}
					_value = _content.Substring(start, _position - start);
					_nextStateLocation = _nextStateLocation.AddCol();
					_position += 2;
					return State.Block;
				}
			}
			//EOF is only valid when we're in content
			_value = _content.Substring(start);
			return State.Eof;
		}

		private int IsNewLine()
		{
			var found = 0;

			if (_position < _content.Length && _content[_position] == '\r')
			{
				found++;
			}
			if (_position + found < _content.Length && _content[_position + found] == '\n')
			{
				found++;
			}
			return found;
		}

		private State NextStateInDirective()
		{
			for (; _position < _content.Length; _position++)
			{
				var c = _content[_position];
				if (c == '\r')
				{
					if (_position + 1 < _content.Length && _content[_position + 1] == '\n')
						_position++;
					_nextStateLocation = _nextStateLocation.AddLine();
				}
				else if (c == '\n')
				{
					_nextStateLocation = _nextStateLocation.AddLine();
				}
				else if (Char.IsLetter(c))
				{
					return State.DirectiveName;
				}
				else if (c == '=')
				{
					_nextStateLocation = _nextStateLocation.AddCol();
					_position++;
					return State.DirectiveValue;
				}
				else if (c == '#' && _position + 1 < _content.Length && _content[_position + 1] == '>')
				{
					_position += 2;
					TagEndLocation = _nextStateLocation.AddCols(2);
					_nextStateLocation = _nextStateLocation.AddCols(3);

					//skip newlines directly after directives
					if ((_position += IsNewLine()) > 0)
					{
						_nextStateLocation = _nextStateLocation.AddLine();
					}

					return State.Content;
				}
				else if (!Char.IsWhiteSpace(c))
				{
					throw new ParserException("Directive ended unexpectedly with character '" + c + "'", _nextStateLocation);
				}
				else
				{
					_nextStateLocation = _nextStateLocation.AddCol();
				}
			}
			throw new ParserException("Unexpected end of file.", _nextStateLocation);
		}

		public State State { get; private set; }

		public int Position
		{
			get { return _position; }
		}

		public string Content
		{
			get { return _content; }
		}

		public string Value
		{
			get { return _value; }
		}

		public Location Location { get; private set; }
		public Location TagStartLocation { get; private set; }
		public Location TagEndLocation { get; private set; }
	}

	public enum State
	{
		Content = 0,
		Directive,
		Expression,
		Block,
		Helper,
		DirectiveName,
		DirectiveValue,
		Name,
		Eof
	}

	public class ParserException : Exception
	{
		public ParserException(string message, Location location) : base(message)
		{
			Location = location;
		}

		public Location Location { get; private set; }
	}
}
