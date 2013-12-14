using System;
using System.Collections.Generic;
using System.Text;
using Guineu.ObjectEngine;

namespace Guineu.Gui
{
	interface IGuiColumn
	{
		void GuiPrepareColumn(basColumn col);
		// (...) Des kann's net sein.
		void GuiUpdateColumn();
	}
}
