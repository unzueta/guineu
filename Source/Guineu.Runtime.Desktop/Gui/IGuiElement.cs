using System;
using System.ComponentModel;
using System.Drawing;

namespace Guineu.Gui
{
	public interface 
    IGuiElement : IDisposable, IControl
	{
		void GuiAddControl( IGuiElement ctrl );
		void GuiBringToFront();

		// Point GuiLocation { get;set;}
		// Font GuiFont { get;set;}
		// String GuiText { get;set;}
		//Int32 GuiLeft { get;set;}
		//Int32 GuiTop { get;set;}
		//Int32 GuiWidth { get;set;}
		//Int32 GuiHeight { get;set;}
		// Boolean GuiEnabled { get;set;}
		//Color GuiBackColor { get; set; }
		// Color GuiForeColor { get; set; }
		//Color GuiDisabledBackColor { get; set; }
		//Color GuiDisabledForeColor { get; set; }
		// Boolean GuiVisible { get; set; }
		// Int32 BackStyle { get; set;}
		// String GuiPicture { get; set; }

		//event EventHandler GuiClick;
		//event EventHandler GuiGotFocus;
		//event EventHandler GuiLostFocus;
		//event CancelEventHandler GuiValid;
		//event CancelEventHandler GuiWhen;
	}
}
