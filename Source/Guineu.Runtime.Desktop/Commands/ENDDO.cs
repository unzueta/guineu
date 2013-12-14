using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu
{

	class ENDDO : ICommand
	{
		int m_Jump;

		public void Compile(CodeBlock code)
		{
			m_Jump = code.ControlFlowStack.Pop(FlowControlEntry.Types.DoWhile);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			nextLine = m_Jump;
		}
	}

}
