using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Guineu.Expression;
using Guineu.Properties;

namespace Guineu
{
	partial class MessageboxFunction : ExpressionBase
	{
		private static DialogResult ShowDefault(string Message, Int32 options, String Caption)
		{
			// default button
			MessageBoxDefaultButton defBtn;
			switch (options & 0x300)
			{
				case 512:
					defBtn = MessageBoxDefaultButton.Button3;
					break;
				case 256:
					defBtn = MessageBoxDefaultButton.Button2;
					break;
				default:
					defBtn = MessageBoxDefaultButton.Button1;
					break;
			}

			// icons
			MessageBoxIcon icon;
			switch (options & 0xF8)
			{
				case 16:
					icon = MessageBoxIcon.Hand;
					break;
				case 32:
					icon = MessageBoxIcon.Question;
					break;
				case 48:
					icon = MessageBoxIcon.Exclamation;
					break;
				case 64:
					icon = MessageBoxIcon.Asterisk;
					break;
				default:
					icon = MessageBoxIcon.None;
					break;
			}

			MessageBoxButtons btn;
			switch (options & 0x07)
			{
				case 0:
					btn = MessageBoxButtons.OK;
					break;
				case 1:
					btn = MessageBoxButtons.OKCancel;
					break;
				case 2:
					btn = MessageBoxButtons.AbortRetryIgnore;
					break;
				case 3:
					btn = MessageBoxButtons.YesNoCancel;
					break;
				case 4:
					btn = MessageBoxButtons.YesNo;
					break;
				case 5:
					btn = MessageBoxButtons.RetryCancel;
					break;
				default:
					btn = MessageBoxButtons.OK;
					break;
			}
			DialogResult result = MessageBox.Show((Control)null, Message, Caption, btn, icon, defBtn, (MessageBoxOptions)0);
			return result;
		}
	}
}