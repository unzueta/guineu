using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class OTHERWISE : ICommand
	{
		Int32 m_LineInfo;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			Comp.ReadInt();
			m_LineInfo = code.ControlFlowStack.Pop(FlowControlEntry.Types.DoCase);
			code.ControlFlowStack.Push(m_LineInfo, FlowControlEntry.Types.DoCase);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			LineInfoCase info = (LineInfoCase)context.GetLineInfo(m_LineInfo);
			if (info.Executed)
				nextLine = info.JumpEnd;
		}

	}

}
