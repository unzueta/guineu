using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;
using Guineu.Expression;
using Guineu.Gui;
using Guineu;

namespace Guineu.Gui.WebGUI
{
	public class WebGUIScreen : Form
	{
		public WebGUIScreen()
		{
			IsMdiContainer = true;
			String homeDir = VWGContext.Current.Server.MapPath("~");
			Directory.SetCurrentDirectory(homeDir);
			GuineuInstance.WinMgr = new WebGUIManager();
			((Guineu.Gui.WebGUI.WebGUIManager)GuineuInstance.WinMgr).mainForm = this;
		}
	}

	class WebGUIManager : WindowManager
	{
		internal Form mainForm;

		public WebGUIManager()
		{
			Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
		}
		void Application_ApplicationExit(object sender, EventArgs e)
		{
			GuineuInstance.Quit();
		}

		public override IGuiElement CreateTextBox()
		{
			return new WebGUITextbox();
		}
		public override IGuiElement CreateForm()
		{
			WebGUIForm frm;
			frm = new WebGUIForm();
			frm.MdiParent = mainForm;
			return frm;
		}
		public override IGuiElement CreateButton()
		{
			return new WebGUIButton();
		}
		public override IGuiElement CreateTabPage()
		{
			return new WebGUITabPage();
		}
		public override IGuiElement CreateListBox()
		{
			return new WebGUIListbox();
		}
		public override IGuiElement CreateShape()
		{
			return new WebGuiShape();
		}
		public override IGuiElement CreateImage()
		{
			return new WebGUIImage();
		}
		public override IGuiElement CreateLabel()
		{
			return new WebGUILabel();
		}
		public override IGuiElement CreateContainer()
		{
			return new WebGUIContainer();
		}
		public override IGuiElement CreateEditbox()
		{
			return new WebGuiEditbox();
		}
		public override IGuiElement CreateCheckBox()
		{
			return new WebGUICheckBox();
		}
		public override IGuiElement CreatePageframe()
		{
			return new WebGUIPageframe();
		}
		public override IGuiElement CreateComboBox()
		{
			return new WebGuiComboBox();
		}
		public override IGuiElement CreateColumn()
		{
			throw new NotImplementedException();
		}
		public override IGuiElement CreateGrid()
		{
			throw new NotImplementedException();
		}
		public override IGuiElement CreateHeader()
		{
			throw new NotImplementedException();
		}
		public override IGuiElement CreateSpinner()
		{
			return new WebGUISpinner();
		}
		public override IGuiElement Create(Guineu.Expression.KnownNti nti)
		{
			throw new ErrorException(ErrorCodes.ClassDefinitionNotFound);
		}

		public override IControl CreateControl(KnownNti name)
		{
			switch (name)
			{
				default:
					throw new ErrorException(ErrorCodes.ClassDefinitionNotFound);
			}
		}

		public override void ClearEvents()
		{
		}
		public override void ReadEvents()
		{
		}

		//======================================================================================
		// window handling functions
		//======================================================================================
		public override Window GetWindowByName(String name)
		{
			// TODO: Access form collection
			return null;
		}
		public override Window CreateWindow()
		{
			return new GUIWindow(null);
		}

		public override TableOpenDialogResult ShowOpenTableDialog()
		{
			OpenFileDialog dlg = new OpenFileDialog();
			TableOpenDialogResult result = new TableOpenDialogResult();
			if (dlg.ShowDialog() == DialogResult.OK)
				result.TableName = dlg.FileName;
			return result;
		}
		public override string ShowOpenFileDialog()
		{
			OpenFileDialog dlg = new OpenFileDialog();
			if (dlg.ShowDialog() == DialogResult.OK)
				return dlg.FileName;
			return "";
		}
		public override ErrorAction ShowErrorDialog(string text)
		{
			return ErrorAction.Ignore;
		}

		//======================================================================================
		// screen output
		//======================================================================================
		public override void Write(string str)
		{
			foreach (Char ch in str)
			{
				if (ch == 7)
				{
					Beep();
				}
				else
					Console.Write(ch);
			}
		}
		public override void WriteLine()
		{
			Console.WriteLine();
		}
		public override void Beep()
		{
			if (GuineuInstance.Set.Bell)
			{
				if (GuineuInstance.Set.BellFile == null)
					Console.Write((Char)7);
				else
				{
					System.Media.SoundPlayer myPlayer = new System.Media.SoundPlayer();
					myPlayer.SoundLocation = GuineuInstance.Set.BellFile;
					myPlayer.Play();
				}
			}
		}

		public override int Wait(string text, int timeout, double X, double Y, bool window, bool clear)
		{
			// (...) not implemented
			return 0;
		}
	}
	class GUIWindow : Window
	{
		String windowName;
		Form linkedToForm;

		public GUIWindow(Form theForm)
		{
			this.linkedToForm = theForm;
		}

		public override void PutStr(string str)
		{
			Console.Write(str);
		}
		public String Name
		{
			get { return windowName; }
			set { windowName = value; }
		}
		public override Boolean Visible
		{
			get { return linkedToForm.Visible; }
		}
	}

}
