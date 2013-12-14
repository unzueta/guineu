using System;
using System.Windows.Forms;
using Guineu.Expression;

namespace Guineu.Gui.Compact
{
	class CompactListbox : ListBox, IControl, IGuiList
	{
		/*
		 When programming on the .Net Framework (on the Compact Framework especially), it is important to know that the ListBox.SelectedIndexChanged event occurs even when the SelectedIndex property is changed programmatically. That means if you set the SelectedIndex property when you are initializing the control, for example, form Load, this event will fire.

This is a significant behavioral difference from the Click event, which is unsupported by .NET CF. You have to design your code to ignore this event when setting SelectedIndex manually so you don't accidentally treat it as a legitimate selection by the user.

This is not an equivalent substitution for Click.
		 * 
		 * Source: http://msdn.microsoft.com/en-us/library/system.windows.forms.listbox.selectedindexchanged(v=vs.80).aspx
		 */

		Boolean fireInteractiveChange = true;

		public event EventHandler GuiListInteractiveChange;
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			base.OnSelectedIndexChanged(e);
			if (fireInteractiveChange && GuiListInteractiveChange != null)
				GuiListInteractiveChange(this, e);
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
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (e.KeyCode == Keys.Enter)
			{
				var parm = new ParameterCollection { new Variant(13, 10), new Variant(0, 10) };
				e.Handled = this.HandleEvent(EventHandler, KnownNti.KeyPress, parm);
			}
		}

		String rowSource;
		Int32 rowSourceType;
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

				case KnownNti.RowSource:
					rowSource = value;
					CompactListHandling.LoadListData(Items, rowSource, rowSourceType);
					break;

				case KnownNti.RowSourceType:
					rowSourceType = value;
					CompactListHandling.LoadListData(Items, rowSource, rowSourceType);
					break;

				case KnownNti.TabIndex:
					TabIndex = value;
					break;

				case KnownNti.Value:
					type = value.Type;
					fireInteractiveChange = false;
					Text = value;
					fireInteractiveChange = true;
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

				case KnownNti.RowSource:
					return new Variant(rowSource);

				case KnownNti.RowSourceType:
					return new Variant(rowSourceType, 10);

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
	}
}
