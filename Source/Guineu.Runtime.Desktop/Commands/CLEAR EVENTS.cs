using System;

namespace Guineu
{

	class CLEAREVENTS : ICommand
	{

		public void Compile(CodeBlock code)
		{
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			GuineuInstance.WinMgr.ClearEvents();
		}

	}

}
