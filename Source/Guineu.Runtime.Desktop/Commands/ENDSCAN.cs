using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu
{

	class ENDSCAN : ICommand
	{
		int m_Jump;

		public void Compile(CodeBlock code)
		{
			m_Jump = code.ControlFlowStack.Pop(FlowControlEntry.Types.Scan);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			nextLine = m_Jump;
		}
	}

}
