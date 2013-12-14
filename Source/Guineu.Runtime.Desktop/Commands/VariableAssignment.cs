using System;
using Guineu.Expression;

namespace Guineu.Commands
{
	class VariableAssignment
	{
		ExpressionBase destVar;
		ExpressionBase expression;
		readonly Variant value;
		readonly Boolean usesVariant;

		public VariableAssignment(ExpressionBase var, ExpressionBase expr)
			: this()
		{
			destVar = var;
			expression = expr;
			usesVariant = false;
		}
		public VariableAssignment(ExpressionBase var, Variant expr)
			: this()
		{
			destVar = var;
			value = expr;
			usesVariant = true;
		}

		public VariableAssignment() { }

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			destVar = comp.GetCompiledExpression();
			code.Reader.ReadToken();
			expression = comp.GetCompiledExpression();
		}

		public int Do(CallingContext context)
		{
			ValueMember dest = destVar.GetValueMember(context);

			// If a variable doesn't exist, create one on the fly.
			if (dest == null)
			{
				if (destVar is ArrayDefinition)
					throw new ErrorException(ErrorCodes.VariableNotFound);
				dest = context.CreateVariable(destVar as Variable, destVar.ToNti(context));
			}
			Variant val;
			if (usesVariant)
				val = value;
			else
			{
				val = expression.GetVariant(context);
			}
			dest.Set(val);

			//------------------------------------------------------------------------------------
			// When SET TALK ON and SET STATUS BAR OFF, we display all assignments
			//------------------------------------------------------------------------------------
			if (GuineuInstance.Set.Talk.Value && !GuineuInstance.Set.StatusBar.Value)
			{
				Console.WriteLine(dest.Get().ToString(context.Context));
			}

			////------------------------------------------------------------------------------------
			//// If the destination supports the IProgrammaticChangeEvent interface, we notify the
			//// object that this change was initiated by program code.
			////------------------------------------------------------------------------------------
			var obj = dest as IProgrammaticChangeEvent;
			if (obj != null)
			{
				obj.OnProgrammaticChange();
			}

			return -1;
		}
	}
	public interface IProgrammaticChangeEvent
	{
		void OnProgrammaticChange();
	}

}

