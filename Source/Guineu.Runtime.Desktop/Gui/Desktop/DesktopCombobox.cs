using System;
using System.Windows.Forms;
using System.ComponentModel;
using Guineu.Expression;

namespace Guineu.Gui.Desktop
{
    class DesktopComboBox : ComboBox, IControl, IGuiList
    {
        Int32 lastIndex;
        Object	lastValue;

        public event EventHandler GuiListInteractiveChange;

        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);
            this.ValidEvent(this, EventHandler, GetValue());
        }
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            this.WhenEvent(this, EventHandler);
        }
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
            base.OnGotFocus(e);
            this.CallEvent(EventHandler, KnownNti.GotFocus);
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.CallEvent(EventHandler, KnownNti.LostFocus);
        }

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

        #region IGuiList Members


        public int GuiListCount
        {
            get
            {
                return Items.Count;
            }
            set
            {
                lastIndex = value;
                new ErrorException(ErrorCodes.PropertyIsReadOnly);
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
            get { return Text; }
            set { Text = value; }
        }

        #endregion

        protected override void OnSelectedValueChanged(EventArgs e)
        {
            if (readOnly)
            {
                SelectedIndex = lastIndex;
                SelectedValue = lastValue;
            }
            base.OnSelectedValueChanged(e);
        }

        String rowSource;
        Int32 rowSourceType;
        Boolean readOnly;

        public void SetVariant(KnownNti nti, Variant value)
        {
            switch (nti)
            {
                case KnownNti.BackColor:
                    BackColor = new Color(value);
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

                case KnownNti.ReadOnly:
                    readOnly = value;
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

                    throw new ErrorException(ErrorCodes.PropertyIsNotFound, nti);
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
                    throw new ErrorException(ErrorCodes.PropertyIsNotFound, name);
            }
        }

        public event Action<EventData> EventHandler;

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

    }
}
