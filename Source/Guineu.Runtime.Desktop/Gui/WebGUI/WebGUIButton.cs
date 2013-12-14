using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.ComponentModel;

using Gizmox.WebGUI.Common;
using Gizmox.WebGUI.Forms;
using Gizmox.WebGUI.Common.Resources;

using Guineu.Util;
using Guineu.Expression;


namespace Guineu.Gui.WebGUI
{
	class WebGUIButton : Button, IGuiElement, IGuiButton, IGuiProperty
	{
		internal WebGUIButton()
		{
		  this.Click += new System.EventHandler(this.handleClick);
		}

		#region additional state information
		String curDownPicture;
		String curPicture;
		#endregion

		#region Internal helpers

		void ShowPicture()
		{
			String picture;
			picture = curPicture;

			if (String.IsNullOrEmpty(picture))
				Image = null;
			else
			{
				Image = new ImageResourceHandle(picture);
			}
		}
		
#endregion

		#region Event handling code

		private void handleClick(object sender, EventArgs e)
		{
		  if (GuiClick != null)
		    GuiClick(this, e);
		}	

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			if (GuiGotFocus != null)
				GuiGotFocus(this, e);
		}
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			if (GuiLostFocus != null)
				GuiLostFocus(this, e);
		}
		//protected override void OnMouseDown(MouseEventArgs mevent)
		//{
		//  // TODO: Implement NODEFAULT in MouseDown event
		//  // TODO: Implement change of picture when mouse is moved off the
		//  //       button. Note: This doesn't raise the mouse move events as
		//  //       it does in VFP.
		//  isMouseDown = true;
		//  base.OnMouseDown(mevent);
		//  this.ShowPicture();
		//}
		//protected override void OnMouseUp(MouseEventArgs mevent)
		//{
		//  isMouseDown = false;
		//  this.ShowPicture();
		//  base.OnMouseUp(mevent);
		//}
		protected override void OnValidating(System.ComponentModel.CancelEventArgs e)
		{
			base.OnValidating(e);
			if (GuiValid != null)
				GuiValid(this, e);
		}
		#endregion

		#region Events

		public event EventHandler GuiClick;
		public event EventHandler GuiGotFocus;
		public event EventHandler GuiLostFocus;
		public event CancelEventHandler GuiValid;
		public event CancelEventHandler GuiWhen;

		#endregion

		#region IGuiElement Members

		public void GuiAddControl(IGuiElement ctrl)
		{
			Controls.Add((Control)ctrl);
		}
		public void GuiBringToFront()
		{
			BringToFront();
		}
		public void GuiSetFocus()
		{
			Focus();
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

		public string GuiPicture
		{
			get 			{				return this.curPicture;			}
			set
			{
				this.curPicture = value;
				ShowPicture();
			}
		}


		
#endregion

		#region IGuiButton Members

		public string GuiDownPicture
		{
			get { return curDownPicture; }
			set
			{
				curDownPicture = value;
				ShowPicture();
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

		#region IGuiButton Members

		/// <summary>
		/// WordWrap is not implemented for WebGUI
		/// </summary>
		Boolean wordWrap;
		public bool GuiWordWrap
		{
			get
			{
				return wordWrap;
			}
			set
			{
				wordWrap = value;
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
