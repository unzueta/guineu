using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class SETDECIMALS: ICommand
	{
		ExpressionBase _Value;

		public void Compile(CodeBlock code)
		{
			var Comp = new Compiler(null, code);
			code.Reader.ReadToken(); // TO
			_Value = Comp.GetCompiledExpression();
		}

		public void Do(CallingContext context, ref int nextLine)
		{
			GuineuInstance.Set.Decimals.Value = _Value.GetInt(context);
		}
	}

	public class SetDecimalsValue
	{
		public Int32 Value { get; set; }
		public SetDecimalsValue()
		{
			Value = 2;
		}
	}

}
