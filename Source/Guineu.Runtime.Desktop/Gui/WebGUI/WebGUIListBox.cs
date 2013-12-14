using System;
using System.Collections.Generic;
using System.Text;
using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;
using System.ComponentModel;
using System.Drawing;
using Guineu.Expression;

namespace Guineu.Gui.WebGUI
{
	class WebGUIListbox : ListBox, IGuiElement, IGuiList, IGuiProperty
	{
		#region Event handling code
		protected override void OnValidating(System.ComponentModel.CancelEventArgs e)
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


		#region Events

		public event CancelEventHandler GuiValid;
		public event CancelEventHandler GuiWhen;
		public event EventHandler GuiListInteractiveChange;

		#endregion

		#region IGuiElement Members

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

		public System.Drawing.Point GuiLocation
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
		public System.Drawing.Font GuiFont
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
		public System.Drawing.Color GuiBackColor
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
		public System.Drawing.Color GuiForeColor
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
		public String GuiPicture
		{
			get { throw new ErrorException(ErrorCodes.PropertyIsNotFound, "PICTURE"); }
			set { throw new ErrorException(ErrorCodes.PropertyIsNotFound, "PICTURE"); }
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
			this.Items.Clear();
		}
		public void GuiAddItem(String item)
		{
			this.Items.Add(item);
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
			set { Font = new Font(Font.Name, (float)value, Font.Style, Font.Unit); }
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
				return this.SelectedIndex + 1;
			}
			set
			{
				this.SelectedIndex = value - 1;
			}
		}
		public string GuiDisplayValue
		{
			get
			{
				return this.Text;
			}
			set
			{
				this.Text = value;
			}
		}

		#endregion

		#region IGuiProperty Members

		public void GuiSetVariant(Guineu.Expression.KnownNti nti, Variant value)
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

		public void GuiSetColor(Guineu.Expression.KnownNti nti, Color color)
		{
			throw new NotImplementedException();
		}

		public Variant GuiGetVariant(Guineu.Expression.KnownNti nti)
		{
			switch (nti)
			{
				case KnownNti.TabIndex:
					return new Variant(TabIndex, 10);

				default:
					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		public Color GuiGetColor(Guineu.Expression.KnownNti nti)
		{
			throw new NotImplementedException();
		}

		#endregion	
	}
}
