using System;
using System.Collections.Generic;
using System.Text;

namespace Guineu.Gui
{
	interface IGuiPageframe
	{
		/// <summary>
		/// Index of the currently active page in a pageframe. First page is 1. 0 means,
		/// no page is activated.
		/// </summary>
		Int32 GuiActivePage { get; set; }
	}
}
