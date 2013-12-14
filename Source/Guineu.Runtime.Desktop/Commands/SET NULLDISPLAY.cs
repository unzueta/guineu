using System;
using Guineu.Expression;

namespace Guineu.Commands
{
	class SETNULLDISPLAY : ICommand
	{
		ExpressionBase value;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);
			code.Reader.ReadToken(); // TO
			value = comp.GetCompiledExpression();
		}

		public void Do(CallingContext context, ref int nextLine)
		{
			GuineuInstance.Set.NullDisplay.Value = value.GetString(context);
		}
	}

	public class SetNullDisplayValue
	{
		String value;

		public String Value
		{
			get
			{
				if (string.IsNullOrEmpty(value))
					return ".NULL.";
				
				return value;
			}

			set { this.value = value; }
		}
	}
}
