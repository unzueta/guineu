using System;
using Gizmox.WebGUI.Forms;
using System.ComponentModel;
using System.Drawing;
using Guineu.Expression;

namespace Guineu.Gui.WebGUI
{
	class WebGuiEditbox : TextBox, IGuiElement, IGuiProperty
	{
		public WebGuiEditbox()
		{
			Multiline = true;
            ScrollBars = ScrollBars.Vertical;
		}

		#region Events

		public event CancelEventHandler GuiValid;
		public event CancelEventHandler GuiWhen;

		#endregion

		#region Event handling code
		protected override void OnValidating(CancelEventArgs e)
		{
			base.OnValidating(e);
			if (GuiValid != null)
				GuiValid(this, e);
		}
		#endregion

		#region IGuiElement Members

		public String GuiPicture
		{
			get { throw new ErrorException(ErrorCodes.PropertyIsNotFound, "PICTURE"); }
			set { throw new ErrorException(ErrorCodes.PropertyIsNotFound, "PICTURE"); }
		}
		public void GuiSetFocus()
		{
			Focus();
		}
		public void GuiAddControl(IGuiElement ctrl)
		{
			Controls.Add((Control)ctrl);
		}
		public void GuiBringToFront()
		{
			BringToFront();
		}

		public Point GuiLocation
		{
			get
			{
				return Location;
			}
			set
			{
				Location = value;
			}
		}
		public Font GuiFont
		{
			get
			{
				return Font;
			}
			set
			{
				Font = value;
			}
		}
		public string GuiText
		{
			get
			{
				return Text;
			}
			set
			{
				Text = value;
			}
		}
		public int GuiLeft
		{
			get
			{
				return Left;
			}
			set
			{
				Left = value;
			}
		}
		public int GuiTop
		{
			get
			{
				return Top;
			}
			set
			{
				Top = value;
			}
		}
		public int GuiWidth
		{
			get
			{
				return Width;
			}
			set
			{
				Width = value;
			}
		}
		public int GuiHeight
		{
			get
			{
				return Height;
			}
			set
			{
				Height = value;
			}
		}
		public bool GuiEnabled
		{
			get
			{
				return Enabled;
			}
			set
			{
				Enabled = value;
			}
		}
		public Color GuiBackColor
		{
			get
			{
				return BackColor;
			}
			set
			{
				BackColor = value;
			}
		}
		public Color GuiForeColor
		{
			get
			{
				return ForeColor;
			}
			set
			{
				ForeColor = value;
			}
		}
		public bool GuiVisible
		{
			get
			{
				return Visible;
			}
			set
			{
				Visible = value;
			}
		}
		public Int32 BackStyle
		{
			get { throw new ErrorException(ErrorCodes.PropertyIsNotFound, "BACKSTYLE"); }
			set { throw new ErrorException(ErrorCodes.PropertyIsNotFound, "BACKSTYLE"); }
		}

		public event EventHandler GuiClick;
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			if (GuiClick != null)
				GuiClick(this, e);
		}
		public event EventHandler GuiGotFocus;
		protected override void OnGotFocus(EventArgs e)
		{
			Boolean enterAllowed = true;
			if (GuiWhen != null)
			{
				var args = new CancelEventArgs();
				GuiWhen(this, args);
				enterAllowed = !args.Cancel;
			}
			if (enterAllowed)
			{
				base.OnGotFocus(e);
				if (GuiGotFocus != null)
					GuiGotFocus(this, e);
			}
				// TODO: Prevent eternal loop
		}

		public event EventHandler GuiLostFocus;
		protected override void OnLostFocus(EventArgs e)
		{
			Boolean raiseLostFocus = true;
			if (GuiValid != null)
			{
				var args = new CancelEventArgs();
				GuiValid(this, args);
				if (args.Cancel)
					Focus();
				raiseLostFocus = !args.Cancel;
			}
			if (raiseLostFocus)
			{
				base.OnLostFocus(e);
				if (GuiLostFocus != null)
					GuiLostFocus(this, e);
			}
		}

		#endregion

		#region IGuiElement members, Font properties
		public string GuiFontName
		{
			get { return Font.Name; }
			set { Font = new Font(value, Font.Size, Font.Style, Font.Unit); }
		}
		public int GuiFontSize
		{
			get { return (Int32)Font.Size; }
			set { Font = new Font(Font.Name, value, Font.Style, Font.Unit); }
		}
		public bool GuiFontBold
		{
			get { return Font.Bold; }
			set
			{
				if (value)
					Font = new Font(Font, Font.Style | FontStyle.Bold);
				else
					Font = new Font(Font, Font.Style & ~FontStyle.Bold);
			}
		}
		public bool GuiFontItalic
		{
			get { return Font.Italic; }
			set
			{
				if (value)
					Font = new Font(Font, Font.Style | FontStyle.Italic);
				else
					Font = new Font(Font, Font.Style & ~FontStyle.Italic);
			}
		}
		public bool GuiFontStrikeThru
		{
			get { return Font.Strikeout; }
			set
			{
				if (value)
					Font = new Font(Font, Font.Style | FontStyle.Strikeout);
				else
					Font = new Font(Font, Font.Style & ~FontStyle.Strikeout);
			}
		}
		public bool GuiFontUnderline
		{
			get { return Font.Underline; }
			set
			{
				if (value)
					Font = new Font(Font, Font.Style | FontStyle.Underline);
				else
					Font = new Font(Font, Font.Style & ~FontStyle.Underline);
			}
		}
		#endregion

		#region IGuiProperty Members

		public void GuiSetVariant(KnownNti nti, Variant value)
		{
			switch (nti)
			{
				case KnownNti.TabIndex:
					TabIndex = value.ToInt();
					break;

				default:
					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		public void GuiSetColor(KnownNti nti, Color color)
		{
			throw new NotImplementedException();
		}

		public Variant GuiGetVariant(KnownNti nti)
		{
			switch (nti)
			{
				case KnownNti.TabIndex:
					return new Variant(TabIndex, 10);

				default:
					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		public Color GuiGetColor(KnownNti nti)
		{
			throw new NotImplementedException();
		}

		#endregion	

		public void SetVariant(KnownNti nti, Variant value)
		{
			lock (this)
			{
				switch (nti)
				{
					case KnownNti.SelStart:
						SelectionStart = value.ToInt();
						break;
					case KnownNti.SelLength:
						SelectionLength = value.ToInt();
						break;
					default:
						throw new ErrorException(ErrorCodes.PropertyIsNotFound);
				}
			}
		}

		public Variant GetVariant(KnownNti nti)
		{
			switch (nti)
			{
				case KnownNti.SelStart:
					return new Variant(SelectionStart, 10);
				case KnownNti.SelLength:
					return new Variant(SelectionLength, 10);
				default:
					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		public Variant CallMethod(KnownNti name, ParameterCollection parms)
		{
			switch (name)
			{
				default:
					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		public event Action<EventData> EventHandler;
	
	}
}
