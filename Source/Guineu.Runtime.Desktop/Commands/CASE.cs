using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class CASE : ICommand
	{
		ExpressionBase m_Condition;
		int m_JumpNext;
		Int32 m_LineInfo;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			m_Condition = Comp.GetCompiledExpression();
			m_JumpNext = code.GetLineAtPosition(Comp.ReadInt());
			m_LineInfo = code.ControlFlowStack.Pop(FlowControlEntry.Types.DoCase);
			code.ControlFlowStack.Push(m_LineInfo, FlowControlEntry.Types.DoCase);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			LineInfoCase info = (LineInfoCase) context.GetLineInfo(m_LineInfo);
			if (info.Executed)
				nextLine = info.JumpEnd;
			else
			{
				info.Executed = m_Condition.GetBool(context);
				if (!info.Executed)
				{
					nextLine = m_JumpNext;
				}
			}
		}

	}

}
