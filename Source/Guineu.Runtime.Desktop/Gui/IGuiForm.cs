using System;
using System.Collections.Generic;
using System.Text;

namespace Guineu.Gui
{
	interface IGuiForm
	{
		void GuiShow();
		void GuiShowDialog();
		String GuiName { set;}
		event EventHandler GuiUnload;
	}
}
