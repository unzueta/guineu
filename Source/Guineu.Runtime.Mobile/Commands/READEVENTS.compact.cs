using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Guineu
{

	partial class READEVENTS : ICommand
	{
		static Form _Event;

		public void DoReadEvents()
		{
			if (_Event == null)
			{
				_Event = new Form();
			}
			System.Windows.Forms.Application.Run(_Event);
		}

	}

}
