using System;
using System.Collections.Generic;
using System.Text;

namespace Guineu.Gui
{
	interface IGuiList
	{
		void GuiClear();
		event EventHandler GuiListInteractiveChange;
		void GuiAddItem(String value);
		void GuiRemoveItem(Int32 item);
		Int32 GuiListCount { get; set; }
		Int32 GuiListIndex { get; set; }
		String GuiDisplayValue { get; set; }
	}
}
