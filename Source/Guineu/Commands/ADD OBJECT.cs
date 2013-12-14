using System;
using System.Collections.Generic;
using Guineu.Commands;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu
{
	class ADDOBJECT : ICommand
	{
		ExpressionBase name;
		ExpressionBase className;
		List<VariableAssignment> properties;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			properties = new List<VariableAssignment>();
			name = comp.GetCompiledExpression();

			Token nextToken;
			do
			{
				nextToken = code.Reader.ReadToken();
				VariableAssignment var;
				switch (nextToken)
				{
					case Token.AS:
						className = comp.GetCompiledExpression();
						break;
					case Token.WITH:
						var = new VariableAssignment();
						var.Compile(code);
						properties.Add(var);
						break;
					case Token.Comma:
						var = new VariableAssignment();
						var.Compile(code);
						properties.Add(var);
						break;
					case Token.CmdEnd:
						break;
					// TODO: Implement PROTECTED clause
					// TODO: Implement NOINIT clause
					default:
						throw new ErrorException(ErrorCodes.Syntax);
				}
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext exec, ref Int32 nextLine)
		{
			String n = name.GetName(exec);
			Nti c = className.ToNti(exec);

			// TODO: Think about how to create the object
			//       Here we first have to add properties then initialize the object!!!

			// Resolve multi-level object names
			var host = ObjectBase.GetActualParent(exec.This, n);
			Nti objName = ObjectBase.GetActualName(n);

			ObjectBase newObj = GuineuInstance.ObjectFactory.AddObject(host, exec, c, objName);
			using (var ctx = new CallingContext(exec.Context, newObj))
			{
				ctx.Resolver = exec.Resolver;
				ctx.Write = newObj;
				foreach (VariableAssignment var in properties)
					var.Do(ctx);
			}
		}
	}
}
