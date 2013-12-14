using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu
{

	class ENDCASE : ICommand
	{

		int m_LineInfo;

		public void Compile(CodeBlock code)
		{
			m_LineInfo = code.ControlFlowStack.Pop(FlowControlEntry.Types.DoCase);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			context.SetLineInfo(m_LineInfo, null);
		}

	}

}
