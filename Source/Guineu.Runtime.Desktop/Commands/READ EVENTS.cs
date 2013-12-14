using System;
using System.Collections.Generic;
using System.IO;

namespace Guineu
{

	partial class READEVENTS : ICommand
	{

		public void Compile(CodeBlock code)
		{
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			GuineuInstance.WinMgr.ReadEvents();
		}
	}
}
