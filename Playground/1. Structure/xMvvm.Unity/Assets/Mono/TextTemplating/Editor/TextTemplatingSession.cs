// 
// TextTemplatingSession.cs
//  
// Author:
//       Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (c) 2010 Novell, Inc.
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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Mono.TextTemplating
{
	[Serializable]
	public sealed class TextTemplatingSession : Dictionary<string, Object>, ITextTemplatingSession
	{
		public TextTemplatingSession() : this(Guid.NewGuid())
		{
		}

		TextTemplatingSession(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			_id = (Guid)info.GetValue("Id", typeof(Guid));
		}

		public TextTemplatingSession(Guid id)
		{
			_id = id;
		}


		public Guid Id => _id;
		private readonly Guid _id;
		
		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is TextTemplatingSession o && o.Equals(this);
		}

		public bool Equals(Guid other)
		{
			return other.Equals(_id);
		}

		public bool Equals(ITextTemplatingSession other)
		{
			return other != null && other.Id == _id;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Id", _id);
		}
	}
}
