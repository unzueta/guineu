using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Guineu.Expression;

namespace Guineu.Gui.Desktop
{
	class DesktopButton : Button, IControl, IGuiButton
	{
		#region additional state information
		String curDownPicture;
		String curPicture;
		Boolean isMouseDown;
		#endregion

		public DesktopButton()
		{
			TextImageRelation = TextImageRelation.ImageAboveText;
		}

		void ShowPicture()
		{
			String picture;
			if (isMouseDown)
				picture = curDownPicture;
			else
				picture = curPicture;

			if (String.IsNullOrEmpty(picture))
				Image = null;
			else
			{
				Stream s = GuineuInstance.FileMgr.Open(
						picture,
						FileMode.Open,
						FileAccess.Read,
						FileShare.Read
				);
				using (s)
					Image = new Bitmap(s);
			}
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			this.CallEvent(EventHandler, KnownNti.Click);
		}
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			this.CallEvent(EventHandler, KnownNti.GotFocus);
		}
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			this.CallEvent(EventHandler, KnownNti.LostFocus);
		}

		protected override void OnMouseDown(MouseEventArgs mevent)
		{
			// TODO: Implement NODEFAULT in MouseDown event
			// TODO: Implement change of picture when mouse is moved off the
			//       button. Note: This doesn't raise the mouse move events as
			//       it does in VFP.
			isMouseDown = true;
			base.OnMouseDown(mevent);
			ShowPicture();
		}
		protected override void OnMouseUp(MouseEventArgs mevent)
		{
			isMouseDown = false;
			ShowPicture();
			base.OnMouseUp(mevent);
		}
		protected override void OnValidating(CancelEventArgs e)
		{
			base.OnValidating(e);
			this.ValidEvent(this, EventHandler);
		}
		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			this.WhenEvent(this, EventHandler);
		}

		public string GuiPicture
		{
			get { return curPicture; }
			set
			{
				curPicture = value;
				ShowPicture();
			}
		}

public string GuiDownPicture
		{
			get { return curDownPicture; }
			set
			{
				curDownPicture = value;
				ShowPicture();
			}
		}

		#region Windows API
		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

		//assorted constants needed
		public static int GWL_STYLE = -16;
		private const UInt32 BS_MULTILINE = 0x00002000;
		#endregion

		public void SetVariant(KnownNti nti, Variant value)
		{
			switch (nti)
			{
				case KnownNti.BackColor:
					BackColor = new Color(value);
					break;

				case KnownNti.Caption:
					Text = value;
					break;

				case KnownNti.Enabled:
					Enabled = value;
					break;

				case KnownNti.ForeColor:
					ForeColor = new Color(value);
					break;

				case KnownNti.Left:
					this.SetLeft(value);
					break;

				case KnownNti.Width:
					this.SetWidth(value);
					break;

				case KnownNti.Top:
					this.SetTop(value);
					break;

				case KnownNti.Height:
					this.SetHeight(value);
					break;

				case KnownNti.Picture:
					curPicture = value;
					ShowPicture();
					break;

				case KnownNti.TabIndex:
					TabIndex = value;
					break;

				case KnownNti.Visible:
					Visible = value;
					break;

				case KnownNti.WordWrap:
					UInt32 style = GetWindowLong(Handle, GWL_STYLE);
					if (value)
						style = style | BS_MULTILINE;
					else
						style = style & (~BS_MULTILINE);
					SetWindowLong(Handle, GWL_STYLE, style);
					break;

				default:
					if (FontHandling.Handles(nti))
					{
					    FontHandling.Set(this, nti, value);
					    break;
					}

					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		public Variant GetVariant(KnownNti nti)
		{
			switch (nti)
			{
				case KnownNti.BackColor:
					return new Variant((Int32)(Color)BackColor, 10);

				case KnownNti.Enabled:
					return new Variant(Enabled);

				case KnownNti.ForeColor:
					return new Variant((Int32)(Color)ForeColor, 10);

				case KnownNti.Left:
					return new Variant(Left, 10);

				case KnownNti.Top:
					return new Variant(Top, 10);

				case KnownNti.Width:
					return new Variant(Width, 10);

				case KnownNti.Height:
					return new Variant(Height, 10);

				case KnownNti.TabIndex:
					return new Variant(TabIndex, 10);

				case KnownNti.Caption:
					return new Variant(Text);

				case KnownNti.Picture:
					return new Variant(curPicture);

				case KnownNti.Visible:
					return new Variant(Visible);

				case KnownNti.WordWrap:
					UInt32 style = GetWindowLong(Handle, GWL_STYLE);
					if ((style | BS_MULTILINE) == style)
						return new Variant(true);
					return new Variant(false);

				default:
					if (FontHandling.Handles(nti))
						return FontHandling.Get(this, nti);

					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		public Variant CallMethod(KnownNti name, ParameterCollection parms)
		{
			switch (name)
			{
				case KnownNti.SetFocus:
					Focus();
					return new Variant(true);

				case KnownNti.Move:
					this.MoveControl(parms);
					return new Variant(true);

				default:
					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		public event Action<EventData> EventHandler;
	}
}
