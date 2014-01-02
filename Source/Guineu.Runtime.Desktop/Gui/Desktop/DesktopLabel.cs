using System;
using System.Windows.Forms;
using System.ComponentModel;
using Guineu.Expression;

namespace Guineu.Gui.Desktop
{
	class DesktopLabel : Label, IControl
	{
		public DesktopLabel()
		{
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, false);
		}
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				if (backStyle == 0)
					cp.ExStyle |= 0x20;  // Turn on WS_EX_TRANSPARENT
				return cp;
			}
		}
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			if (backStyle == 1)
				base.OnPaintBackground(e);
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

		private Int32 backStyle = 1;

		public void SetVariant(KnownNti nti, Variant value)
		{
			switch (nti)
			{
				case KnownNti.BackColor:
					BackColor = new Color(value);
					break;

				case KnownNti.BackStyle:
					backStyle = value;
					UpdateStyles();
					Refresh();
					break;

				case KnownNti.Caption:
					Text = value.ToString();
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

				case KnownNti.TabIndex:
					TabIndex = value;
					break;

				case KnownNti.Visible:
					Visible = value;
					break;

				default:
                    if (FontHandling.Handles(nti))
                    {
                        FontHandling.Set(this, nti, value);
                        break;
                    }

					throw new ErrorException(ErrorCodes.PropertyIsNotFound, nti);
			}
		}

		public Variant GetVariant(KnownNti nti)
		{
			switch (nti)
			{
				case KnownNti.BackColor:
					return new Variant((Int32)(Color)BackColor, 10);

				case KnownNti.BackStyle:
					return new Variant(backStyle, 10);

				case KnownNti.Caption:
					return new Variant(Text);

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

				case KnownNti.Enabled:
					return new Variant(Enabled);

				case KnownNti.TabIndex:
					return new Variant(TabIndex, 10);

				case KnownNti.Visible:
					return new Variant(Visible);

				default:
					if (FontHandling.Handles(nti))
						return FontHandling.Get(this, nti);

					throw new ErrorException(ErrorCodes.PropertyIsNotFound, nti);
			}
		}

		public Variant CallMethod(KnownNti name, ParameterCollection parms)
		{
			switch (name)
			{
				case KnownNti.Move:
					this.MoveControl(parms);
					return new Variant(true);

				default:
					throw new ErrorException(ErrorCodes.PropertyIsNotFound, name);
			}
		}

		public event Action<EventData> EventHandler;
	}
}
