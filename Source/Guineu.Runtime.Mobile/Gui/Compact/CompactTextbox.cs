using System;
using System.Windows.Forms;
using Guineu.Expression;

namespace Guineu.Gui.Compact
{
	class CompactTextbox : TextBox, IControl, IGridHosted
	{

		// This method intercepts the Enter Key
		// signal before the containing Form does
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
				//MoveFocusByOne();
				Parent.SelectNextControl(this, true, true, true, true);

			base.OnKeyUp(e);

			if (e.KeyCode == Keys.Enter)
			{
				var parm = new ParameterCollection { new Variant(13, 10), new Variant(0, 10) };
				e.Handled = this.HandleEvent(EventHandler, KnownNti.KeyPress, parm);
			}
		}

		//void MoveFocusByOne()
		//{
		//  Control curNext = null;
		//  foreach (Control ctrl in Parent.Controls)
		//  {
		//    if (ctrl.TabStop)
		//      if (ctrl.TabIndex > TabIndex)
		//        if (curNext == null)
		//          curNext = ctrl;
		//        else if (ctrl.TabIndex < curNext.TabIndex)
		//          curNext = ctrl;
		//  }
		//  if (curNext == null)
		//    foreach (Control ctrl in Parent.Controls)
		//    {
		//      if (ctrl.TabStop)
		//        if (ctrl.TabIndex < TabIndex)
		//          if (curNext == null)
		//            curNext = ctrl;
		//          else if (ctrl.TabIndex > curNext.TabIndex)
		//            curNext = ctrl;
		//    }
		//  if (curNext != null)
		//  {
		//    curNext.Focus();
		//  }
		//}

		//Int32 TranslateKeyCodes(KeyEventArgs e)
		//{
		//  Int32 keyCode = 0;
		//  return keyCode;
		//}

		//Int32 TranslateModifier(KeyEventArgs e)
		//{
		//  Int32 modifier = 0;
		//  if (e.Alt)
		//    modifier += 4;
		//  if (e.Control)
		//    modifier += 2;
		//  if (e.Shift)
		//    modifier += 1;
		//  return modifier;
		//}
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

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			this.CallEvent(EventHandler, KnownNti.Click);
		}
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			this.LostFocusEvent(this, EventHandler, GetValue());
		}
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			this.GotFocusEvent(this, EventHandler);
		}

		public void SetVariant(KnownNti nti, Variant value)
		{
			switch (nti)
			{
				case KnownNti.BackColor:
					BackColor = new Color(value);
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

				case KnownNti.ReadOnly:
					ReadOnly = value;
					break;

				case KnownNti.TabIndex:
					TabIndex = value;
					break;

				case KnownNti.Value:
					type = value.Type;
					Text = value;
					break;

				case KnownNti.Visible:
					Visible = value;
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

				case KnownNti.Height:
					return new Variant(ScaleDown(Height), 10);

				case KnownNti.ReadOnly:
					return new Variant(ReadOnly);

				case KnownNti.TabIndex:
					return new Variant(TabIndex, 10);

				case KnownNti.Value:
					return GetValue();

				case KnownNti.Visible:
					return new Variant(Visible);

				default:
					if (FontHandling.Handles(nti))
						return FontHandling.Get(this, nti);

					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		VariantType type;
		private Variant GetValue()
		{
			switch (type)
			{
				//case VariantType.Integer:
				//  break;
				//case VariantType.Logical:
				//  break;
				case VariantType.Character:
					return new Variant(Text);
				//case VariantType.Number:
				//  break;
				//case VariantType.Object:
				//  break;
				case VariantType.Date:
					DateTime dt;
					try
					{
						dt = DateTime.Parse(Text);
					}
					catch
					{
						dt = new DateTime(0);
					}
					return new Variant(dt);
				//case VariantType.DateTime:
				//  break;
				//case VariantType.Null:
				//  break;
				//case VariantType.Unknown:
				//  break;
				default:
					return new Variant(Text);
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
	}
}
