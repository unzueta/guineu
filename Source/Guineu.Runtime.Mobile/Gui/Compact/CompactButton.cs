using System;
using System.Windows.Forms;
using Guineu.Core;
using Guineu.Expression;

namespace Guineu.Gui.Compact
{
	class CompactButton : Button, IControl, IGridHosted
	{
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

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (GuineuInstance.Options.ButtonClickOnEnter)
				if (e.KeyData == Keys.Enter)
				{
					this.CallEvent(EventHandler, KnownNti.Click);
					return;
				}
			base.OnKeyUp(e);
		}

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
					Left = ScaleUp(value);
					break;

				case KnownNti.Width:
					Width = ScaleUp(value);
					break;

				case KnownNti.Top:
					Top = ScaleUp(value);
					break;

				case KnownNti.Height:
					Height = ScaleUp(value);
					break;

				case KnownNti.TabIndex:
					TabIndex = value;
					break;

				case KnownNti.Visible:
					Visible = value;
					break;

				case KnownNti.WordWrap:
					UInt32 style = PInvoke.GetWindowLong(Handle, PInvoke.GWL_STYLE);
					if (value)
						style = style | PInvoke.BS_MULTILINE;
					else
						style = style & (~PInvoke.BS_MULTILINE);
					PInvoke.SetWindowLong(Handle, PInvoke.GWL_STYLE, style);
					break;

				case KnownNti.Picture:
				case KnownNti.DownPicture:
					break;

				default:
					if (FontHandling.Handles(nti))
						FontHandling.Set(this, nti, value);
					else
						throw new ErrorException(ErrorCodes.PropertyIsNotFound);
					break;
			}
		}

		public Variant GetVariant(KnownNti nti)
		{
			switch (nti)
			{
				case KnownNti.BackColor:
					return new Variant((Int32)(Color)BackColor, 10);

				case KnownNti.Caption:
					return new Variant(Text);

				case KnownNti.Enabled:
					return new Variant(Enabled);

				case KnownNti.ForeColor:
					return new Variant((Int32)(Color)ForeColor, 10);

				case KnownNti.Left:
					return new Variant(ScaleDown(Left), 10);

				case KnownNti.Top:
					return new Variant(ScaleDown(Top), 10);

				case KnownNti.Width:
					return new Variant(ScaleDown(Width), 10);

				case KnownNti.WordWrap:
					UInt32 style = PInvoke.GetWindowLong(Handle, PInvoke.GWL_STYLE);
					if ((style | PInvoke.BS_MULTILINE) == style)
						return new Variant(true);
					return new Variant(false);

				case KnownNti.Height:
					return new Variant(ScaleDown(Height), 10);

				case KnownNti.TabIndex:
					return new Variant(TabIndex, 10);

				case KnownNti.Visible:
					return new Variant(Visible);

				case KnownNti.Picture:
				case KnownNti.DownPicture:
					return new Variant("");

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
		public void ForwardEvent(Nti name, ParameterCollection param)
		{
			this.RaiseEvent(EventHandler, name, param);
		}

		static Int32 ScaleDown(Int32 hires)
		{
			var mgr = (CompactManager)GuineuInstance.WinMgr;
			var lores = (Int32)Math.Round(hires / mgr.Scale, 0);
			return lores;
		}

		static Int32 ScaleUp(Int32 lores)
		{
			var mgr = (CompactManager)GuineuInstance.WinMgr;
			var hires = (Int32)Math.Round(lores * mgr.Scale, 0);
			return hires;
		}
	}
}
