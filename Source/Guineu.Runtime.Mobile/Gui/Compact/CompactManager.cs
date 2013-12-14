using System;
using Guineu.Expression;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using Guineu.Properties;

namespace Guineu.Gui.Compact
{
	class CompactManager : WindowManager
	{
		public Double Scale = 1;

		public override IControl CreateControl(KnownNti nti)
		{
			switch (nti)
			{
				case KnownNti.CheckBox:
					return new CompactCheckBox();
				case KnownNti.Column:
					return new CompactColumn();
				case KnownNti.ComboBox:
					return new CompactComboBox();
				case KnownNti.CommandButton:
					return new CompactButton();
				case KnownNti.Container:
					return new CompactContainer();
				case KnownNti.EditBox:
					return new CompactEditbox();
				case KnownNti.Form:
					return new CompactForm();
				case KnownNti.Grid:
					return new CompactGrid();
				case KnownNti.Header:
					return new CompactHeader();
				case KnownNti.Image:
					return new CompactImage();
				case KnownNti.Label:
					return new CompactLabel();
				case KnownNti.ListBox:
					return new CompactListbox();
				case KnownNti.Page:
					return new CompactTabPage();
				case KnownNti.PageFrame:
					return new CompactPageframe();
				case KnownNti.Shape:
					return new CompactShape();
				case KnownNti.Spinner:
					return new CompactSpinner();
				case KnownNti.Signature:
					return new CompactSignature();
				case KnownNti.Timer:
					return new CompactTimer();
				case KnownNti.Textbox:
					return new CompactTextbox();
			}
			throw new ErrorException(ErrorCodes.ClassDefinitionNotFound);
		}

		public override void ClearEvents()
		{
			keepRunning = false;
			//Application.Exit();
		}

		static Boolean keepRunning;
		public override void ReadEvents()
		{
			//if (_Event == null)
			//{
			//  _Event = new Form();
			//}
			//Application.Run(_Event);
			keepRunning = true;
			do
			{
				Thread.Sleep(50);
				Application.DoEvents();
			} while (keepRunning);
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
			return new CompactWindow(null);
		}


		public override TableOpenDialogResult ShowOpenTableDialog()
		{
			var dlg = new OpenFileDialog();
			var result = new TableOpenDialogResult();
			if (dlg.ShowDialog().ToDialogResult() == DialogResult.OK)
				result.TableName = dlg.FileName;
			return result;
		}
		public override string ShowOpenFileDialog()
		{
			var dlg = new OpenFileDialog();
			if (dlg.ShowDialog().ToDialogResult() == DialogResult.OK)
				return dlg.FileName;
			return "";
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

		[Flags]
		private enum Flags
		{
			SND_ASYNC = 0x0001,  /* play asynchronously */
			SND_FILENAME = 0x00020000, /* name is file name */
		}

		[DllImport("CoreDll.DLL", EntryPoint = "PlaySound", SetLastError = true)]
		private extern static int PlaySound(string szSound, IntPtr hMod, int flags);

		[DllImport("CoreDll.dll")]
		public static extern void MessageBeep(int code);

			public override void Beep()
		{
			if (GuineuInstance.Set.Bell)
			{
				if (GuineuInstance.Set.BellFile == null)
					MessageBeep(-1);
				else
				{
					PlaySound(GuineuInstance.Set.BellFile, IntPtr.Zero, (int)(Flags.SND_ASYNC | Flags.SND_FILENAME));
				}
			}
		}

		public override ErrorAction ShowErrorDialog(string text)
		{
			var result =
		System.Windows.Forms.MessageBox.Show(
			text,
			Resources.Execution_Error,
			System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore,
			System.Windows.Forms.MessageBoxIcon.Exclamation,
			System.Windows.Forms.MessageBoxDefaultButton.Button1
		);
			switch (result)
			{
				case System.Windows.Forms.DialogResult.Abort:
					return ErrorAction.Cancel;
				case System.Windows.Forms.DialogResult.Retry:
					return ErrorAction.Retry;
				case System.Windows.Forms.DialogResult.Ignore:
					return ErrorAction.Ignore;
			}
			return ErrorAction.Ignore;
		}

		public override DialogResult MessageBox(string msg, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defBtn)
		{
			// TODO: Check implementation. There should be a method implementing MessageBox somewhere
			throw new NotImplementedException();
		}

		public override int Wait(string text, int timeout, double x, double y, Boolean window, bool clear)
		{
			if (timeout < 0)
				return 0;
			System.Windows.Forms.MessageBox.Show(text);
			return 0;
		}
	}


	class CompactWindow : Window
	{
		readonly Form linkedToForm;

		public CompactWindow(Form theForm)
		{
			linkedToForm = theForm;
		}

		public override void PutStr(string str)
		{
			Console.Write(str);
		}

		public String Name { get; set; }

		public override Boolean Visible
		{
			get { return linkedToForm.Visible; }
		}
	}

	internal static class DialogResultExtension
	{
		internal static DialogResult ToDialogResult(this System.Windows.Forms.DialogResult value)
		{
			switch (value)
			{
				case System.Windows.Forms.DialogResult.No:
					return DialogResult.No;
				case System.Windows.Forms.DialogResult.Yes:
					return DialogResult.Yes;
				case System.Windows.Forms.DialogResult.OK:
					return DialogResult.OK;
				default:
					throw new ArgumentOutOfRangeException("value");
			}
		}
	}

}
