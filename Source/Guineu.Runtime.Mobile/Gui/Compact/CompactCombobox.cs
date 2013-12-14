using System;
using System.Windows.Forms;
using Guineu.Expression;

namespace Guineu.Gui.Compact
{
	class CompactComboBox : ComboBox, IControl, IGuiList
	{
		public CompactComboBox()
		{
			DropDownStyle = ComboBoxStyle.DropDown;
		}

		Int32 lastIndex;
		Object lastValue;
		String rowSource;
		Int32 rowSourceType;
		Boolean readOnly;

		public event EventHandler GuiListInteractiveChange;
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			base.OnSelectedIndexChanged(e);
			if (GuiListInteractiveChange != null)
				GuiListInteractiveChange(this, e);
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			this.CallEvent(EventHandler, KnownNti.Click);
		}
		protected override void OnGotFocus(EventArgs e)
		{
			lastIndex = SelectedIndex;
			lastValue = SelectedValue;
			base.OnGotFocus(e);
			this.GotFocusEvent(this, EventHandler);
		}
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			this.LostFocusEvent(this, EventHandler, new Variant(Text));
		}
		protected override void OnSelectedValueChanged(EventArgs e)
		{
			if (readOnly)
			{
				SelectedIndex = lastIndex;
				SelectedValue = lastValue;
			}
			base.OnSelectedValueChanged(e);
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

				case KnownNti.ReadOnly:
					readOnly = value;
					break;

				case KnownNti.TabIndex:
					TabIndex = value;
					break;

				case KnownNti.RowSource:
					rowSource = value;
					ListHandling.LoadListData(Items, rowSource, rowSourceType);
					break;

				case KnownNti.RowSourceType:
					var newRowSourceType = value;
					if (newRowSourceType != rowSourceType)
					{
						SuspendLayout();
						rowSourceType = newRowSourceType;
						ListHandling.LoadListData(Items, rowSource, rowSourceType);
						ResumeLayout();
					}
					break;

				case KnownNti.Value:
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
					return new Variant(readOnly);

				case KnownNti.TabIndex:
					return new Variant(TabIndex, 10);

				case KnownNti.RowSource:
					return new Variant(rowSource);

				case KnownNti.RowSourceType:
					return new Variant(rowSourceType, 10);

				case KnownNti.Value:
					return new Variant(Text);

				case KnownNti.Visible:
					return new Variant(Visible);

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
