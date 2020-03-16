// 
// ToStringHelper.cs
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
	public static class ToStringHelper
	{
		private static readonly object[] FormatProviderAsParameterArray;

		private static IFormatProvider _formatProvider = System.Globalization.CultureInfo.InvariantCulture;

		static ToStringHelper()
		{
			FormatProviderAsParameterArray = new object[] { _formatProvider };
		}

		public static string ToStringWithCulture(object objectToConvert)
		{
			if (objectToConvert == null)
				throw new ArgumentNullException(nameof(objectToConvert));

			if (objectToConvert is IConvertible conv)
				return conv.ToString(_formatProvider);

			if (objectToConvert is string str)
				return str;

			//TODO: implement a cache of types and DynamicMethods
			var mi = objectToConvert.GetType().GetMethod("ToString", new[] { typeof(IFormatProvider) });
			if (mi != null)
				return (string)mi.Invoke(objectToConvert, FormatProviderAsParameterArray);
			return objectToConvert.ToString();
		}

		public static IFormatProvider FormatProvider
		{
			get { return (IFormatProvider)FormatProviderAsParameterArray[0]; }
			set { FormatProviderAsParameterArray[0] = _formatProvider = value; }
		}
	}
}
