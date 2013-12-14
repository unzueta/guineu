using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{

	class RETURN : ICommand
	{
		ExpressionBase m_ReturnValue;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			m_ReturnValue = Comp.GetCompiledExpression();
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			if (m_ReturnValue == null)
			{
				context.Stack.ReturnValue = new Variant(false);
			}
			else
			{
				context.Stack.ReturnValue = m_ReturnValue.GetVariant(context);
			}
			nextLine = -1;
		}
	}

}