// Window manager

using System;
using Guineu.Expression;

namespace Guineu.Gui
{

	//======================================================================================
	// Each instance of Guineu maintains one set of windows.
	//======================================================================================
	abstract public partial class WindowManager
	{
		private Window m_OutputWindow;

		#region Constructors
		protected WindowManager()
		{
			m_OutputWindow = this.CreateWindow();
		}
		#endregion

public abstract IControl CreateControl(KnownNti name);
		abstract public void ReadEvents();
		abstract public void ClearEvents();

		//======================================================================================
		// window handling functions
		//======================================================================================
		abstract public Window GetWindowByName(String name);
		public Window Active
		{
			get { return m_OutputWindow; }
		}
		public abstract Window CreateWindow();

		//======================================================================================
		// Dialogs
		//======================================================================================
		abstract public TableOpenDialogResult ShowOpenTableDialog();
		abstract public String ShowOpenFileDialog();
		abstract public ErrorAction ShowErrorDialog(String text);

        public DialogResult MessageBox(String msg, String caption,
                                        MessageBoxButtons buttons,
                                        MessageBoxIcon icon)
        {
            return MessageBox(msg, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        public abstract DialogResult MessageBox(String msg, String caption, 
                                                MessageBoxButtons buttons,
												MessageBoxIcon icon,
												MessageBoxDefaultButton defBtn);

		//======================================================================================
		// WAIT
		//======================================================================================
		abstract public Int32 Wait(
				String text, Int32 timeout, Double X, Double Y, Boolean window, Boolean clear);

		//======================================================================================
		// screen output
		//======================================================================================
		abstract public void WriteLine();
		abstract public void Write(String str);
		abstract public void Beep();

		//======================================================================================
		// Color handling
		//======================================================================================
		//        public static ColorCollection SystemColors;
	}

	//======================================================================================
	//======================================================================================
	abstract public class Window
	{
		public virtual void PutStr(string str) { }
		abstract public Boolean Visible { get; }
	}

	public struct TableOpenDialogResult
	{
		public Boolean Selected;
		public String TableName;
		public Boolean Exclusive;
		public Boolean ReadOnly;
	}

	public enum MessageBoxButtons
	{
		YesNo
	}

	public enum MessageBoxIcon
	{
		Question
	}

	public enum MessageBoxDefaultButton
	{
		Button1
	}

	public enum DialogResult
	{
		Yes,
		No,
		OK
	}
}