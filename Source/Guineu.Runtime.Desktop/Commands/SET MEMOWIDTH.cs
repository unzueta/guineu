using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
	class SETMEMOWIDTH : ICommand
	{
		ExpressionBase _Value;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			code.Reader.ReadToken(); // TO
			_Value = Comp.GetCompiledExpression();
		}

		public void Do(CallingContext context, ref int nextLine)
		{
			GuineuInstance.Set.MemoWidth.Value = _Value.GetInt(context);
		}
	}

	public class SetMemoWidthValue
	{
		Int32 _Value = 50;
	
		public Int32 Value
		{
			get { return _Value; }
			set { _Value = value; }
		}
	}

}
