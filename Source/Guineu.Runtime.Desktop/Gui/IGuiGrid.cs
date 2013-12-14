using Guineu.Data;

namespace Guineu.Gui
{
	interface IGuiGrid
	{
		void GuiPreInit();
		void GuiPostInit();
		ICursor GuiRecordSource { get; set; }
	}
}

