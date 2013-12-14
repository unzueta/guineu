using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Commands;
using Guineu.Expression;

namespace Guineu
{

	class ENDFOR : ICommand
	{
		int m_Jump;
	
		public void Compile(CodeBlock code)
		{
			m_Jump = code.ControlFlowStack.Pop(FlowControlEntry.Types.For);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			LineInfoFor info = context.GetLineInfo(m_Jump) as LineInfoFor;
			if (info != null)
			{
				nextLine = m_Jump;				
			}
		}
	}

}
