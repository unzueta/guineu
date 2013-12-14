using System;
using Gizmox.WebGUI.Forms;
using System.ComponentModel;
using System.Drawing;
using Guineu.Expression;

namespace Guineu.Gui.WebGUI
{
	class WebGuiComboBox : ComboBox, IGuiElement, IGuiList, IGuiProperty
	{
		#region Events

		public event CancelEventHandler GuiValid;
		public event CancelEventHandler GuiWhen;
		public event EventHandler GuiListInteractiveChange;
		#endregion

		#region Event handling code
		protected override void OnValidating(CancelEventArgs e)
		{
			base.OnValidating(e);
			if (GuiValid != null)
				GuiValid(this, e);
		}
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			base.OnSelectedIndexChanged(e);
			if (GuiListInteractiveChange != null)
				GuiListInteractiveChange(this, e);
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
			base.OnGotFocus(e);
			if (GuiGotFocus != null)
				GuiGotFocus(this, e);
		}
		public event EventHandler GuiLostFocus;
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			if (GuiLostFocus != null)
				GuiLostFocus(this, e);
		}

		#endregion

		#region IGuiList Members

		public void GuiClear()
		{
			Items.Clear();
		}
		public void GuiAddItem(String item)
		{
			Items.Add(item);
		}
		public void GuiRemoveItem(Int32 item)
		{
			Items.RemoveAt(item);
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

		#region IGuiList Members


		public int GuiListCount
		{
			get
			{
				return Items.Count;
			}
			set
			{
				throw new ErrorException(ErrorCodes.PropertyIsReadOnly);
			}
		}
		public int GuiListIndex
		{
			get
			{
				return SelectedIndex + 1;
			}
			set
			{
				SelectedIndex = value - 1;
			}
		}

		public string GuiDisplayValue
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
	}
}
