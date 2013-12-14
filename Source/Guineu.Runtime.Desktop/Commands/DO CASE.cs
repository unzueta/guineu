using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	class LineInfoCase : LineInfo
	{
		internal Boolean Executed;
		internal Int32 JumpEnd;
	}

	class DOCASE : ICommand
	{
		int m_JumpEnd;
		int m_LineInfo;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			Comp.ReadInt();
			m_JumpEnd = code.GetLineAtPosition(Comp.ReadInt());
			m_LineInfo = code.CurrentLine;
			code.ControlFlowStack.Push(m_LineInfo, FlowControlEntry.Types.DoCase);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			LineInfoCase info = new LineInfoCase();
			info.JumpEnd = m_JumpEnd;
			context.SetLineInfo(m_LineInfo, info);
		}

	}

}
