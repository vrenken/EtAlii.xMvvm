// 
// ParameterDirectiveProcessor.cs
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

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

namespace Mono.TextTemplating
{
	public sealed class ParameterDirectiveProcessor : DirectiveProcessor, IRecognizeHostSpecific
	{
		private CodeDomProvider _provider;

		private bool _hostSpecific;
		private readonly List<CodeStatement> _postStatements = new List<CodeStatement>();
		private readonly List<CodeTypeMember> _members = new List<CodeTypeMember>();

		public override void StartProcessingRun(CodeDomProvider languageProvider, string templateContents, CompilerErrorCollection errors)
		{
			base.StartProcessingRun(languageProvider, templateContents, errors);
			_provider = languageProvider;
			_postStatements.Clear();
			_members.Clear();
		}

		public override void FinishProcessingRun()
		{
			var statement = new CodeConditionStatement(
				new CodeBinaryOperatorExpression(
					new CodePropertyReferenceExpression(
						new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Errors"), "HasErrors"),
					CodeBinaryOperatorType.ValueEquality,
					new CodePrimitiveExpression(false)),
				_postStatements.ToArray());

			_postStatements.Clear();
			_postStatements.Add(statement);
		}

		public override string GetClassCodeForProcessingRun()
		{
			return TemplatingEngine.GenerateIndentedClassCode(_provider, _members);
		}

		public override string[] GetImportsForProcessingRun()
		{
			return null;
		}

		public override string GetPostInitializationCodeForProcessingRun()
		{
			return TemplatingEngine.IndentSnippetText(_provider, StatementsToCode(_postStatements), "            ");
		}

		public override string GetPreInitializationCodeForProcessingRun()
		{
			return null;
		}

		private string StatementsToCode(List<CodeStatement> statements)
		{
			var options = new CodeGeneratorOptions();
			using (var sw = new StringWriter())
			{
				foreach (var statement in statements)
					_provider.GenerateCodeFromStatement(statement, sw, options);
				return sw.ToString();
			}
		}

		public override string[] GetReferencesForProcessingRun()
		{
			return null;
		}

		public override bool IsDirectiveSupported(string directiveName)
		{
			return directiveName == "parameter";
		}

		public override void ProcessDirective(string directiveName, IDictionary<string, string> arguments)
		{
			var name = arguments["name"];
			var type = arguments["type"];
			if (string.IsNullOrEmpty(name))
				throw new DirectiveProcessorException("Parameter directive has no name argument");
			if (string.IsNullOrEmpty(type))
				throw new DirectiveProcessorException("Parameter directive has no type argument");

			var fieldName = "_" + name + "Field";
			var typeRef = new CodeTypeReference(type);
			var thisRef = new CodeThisReferenceExpression();
			var fieldRef = new CodeFieldReferenceExpression(thisRef, fieldName);

			var property = new CodeMemberProperty()
			{
				Name = name,
				Attributes = MemberAttributes.Public | MemberAttributes.Final,
				HasGet = true,
				HasSet = false,
				Type = typeRef
			};
			property.GetStatements.Add(new CodeMethodReturnStatement(fieldRef));
			_members.Add(new CodeMemberField(typeRef, fieldName));
			_members.Add(property);

			var acquiredName = "_" + name + "Acquired";
			var valRef = new CodeVariableReferenceExpression("data");
			var namePrimitive = new CodePrimitiveExpression(name);
			var sessionRef = new CodePropertyReferenceExpression(thisRef, "Session");
			var callContextTypeRefExpr = new CodeTypeReferenceExpression("System.Runtime.Remoting.Messaging.CallContext");
			var nullPrim = new CodePrimitiveExpression(null);

			var acquiredVariable = new CodeVariableDeclarationStatement(typeof(bool), acquiredName, new CodePrimitiveExpression(false));
			var acquiredVariableRef = new CodeVariableReferenceExpression(acquiredVariable.Name);
			_postStatements.Add(acquiredVariable);

			//checks the local called "data" can be cast and assigned to the field, and if successful, sets acquiredVariable to true
			var checkCastThenAssignVal = new CodeConditionStatement(
				new CodeMethodInvokeExpression(
					new CodeTypeOfExpression(typeRef), "IsAssignableFrom", new CodeMethodInvokeExpression(valRef, "GetType")),
				new CodeStatement[]
				{
					new CodeAssignStatement(fieldRef, new CodeCastExpression(typeRef, valRef)),
					new CodeAssignStatement(acquiredVariableRef, new CodePrimitiveExpression(true)),
				},
				new CodeStatement[]
				{
					new CodeExpressionStatement(new CodeMethodInvokeExpression(thisRef, "Error",
						new CodePrimitiveExpression("The type '" + type + "' of the parameter '" + name +
													"' did not match the type passed to the template"))),
				});

			//tries to gets the value from the session
			var checkSession = new CodeConditionStatement(
				new CodeBinaryOperatorExpression(NotNull(sessionRef), CodeBinaryOperatorType.BooleanAnd,
					new CodeMethodInvokeExpression(sessionRef, "ContainsKey", namePrimitive)),
				new CodeVariableDeclarationStatement(typeof(object), "data", new CodeIndexerExpression(sessionRef, namePrimitive)),
				checkCastThenAssignVal);

			_postStatements.Add(checkSession);

			//if acquiredVariable is false, tries to gets the value from the host
			if (_hostSpecific)
			{
				var hostRef = new CodePropertyReferenceExpression(thisRef, "Host");
				var checkHost = new CodeConditionStatement(
					BooleanAnd(IsFalse(acquiredVariableRef), NotNull(hostRef)),
					new CodeVariableDeclarationStatement(typeof(string), "data",
						new CodeMethodInvokeExpression(hostRef, "ResolveParameterValue", nullPrim, nullPrim, namePrimitive)),
					new CodeConditionStatement(NotNull(valRef), checkCastThenAssignVal));

				_postStatements.Add(checkHost);
			}

			//if acquiredVariable is false, tries to gets the value from the call context
			var checkCallContext = new CodeConditionStatement(
				IsFalse(acquiredVariableRef),
				new CodeVariableDeclarationStatement(typeof(object), "data",
					new CodeMethodInvokeExpression(callContextTypeRefExpr, "LogicalGetData", namePrimitive)),
				new CodeConditionStatement(NotNull(valRef), checkCastThenAssignVal));

			_postStatements.Add(checkCallContext);
		}

		private static CodeBinaryOperatorExpression NotNull(CodeExpression reference)
		{
			return new CodeBinaryOperatorExpression(reference, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
		}

		private static CodeBinaryOperatorExpression IsFalse(CodeExpression expr)
		{
			return new CodeBinaryOperatorExpression(expr, CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(false));
		}

		private static CodeBinaryOperatorExpression BooleanAnd(CodeExpression expr1, CodeExpression expr2)
		{
			return new CodeBinaryOperatorExpression(expr1, CodeBinaryOperatorType.BooleanAnd, expr2);
		}

		void IRecognizeHostSpecific.SetProcessingRunIsHostSpecific(bool hostSpecific)
		{
			this._hostSpecific = hostSpecific;
		}

		public bool RequiresProcessingRunIsHostSpecific
		{
			get { return false; }
		}
	}
}
