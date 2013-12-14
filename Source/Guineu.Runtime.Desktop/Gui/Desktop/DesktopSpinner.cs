using System;
using System.Windows.Forms;
using System.ComponentModel;
using Guineu.Expression;

namespace Guineu.Gui.Desktop
{
    class DesktopSpinner : NumericUpDown, IControl
    {
        VariantType type;
        // This method intercepts the Enter Key
        // signal before the containing Form does
        protected override bool ProcessCmdKey(ref 
              Message m,
                            Keys k)
        {
            // detect the pushing (Msg) of Enter Key (k)
            if (m.Msg == 256 && k ==
                         Keys.Enter)
            {
                Parent.SelectNextControl(this, true, true, true, true);
                return true;
            }
            // if not pushing Enter Key,
            // then process the signal as usual
            return base.ProcessCmdKey(ref m, k);
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);
            this.ValidEvent(this, EventHandler, ValueHandling.TextToValue(Text,type));
        }
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            this.WhenEvent(this, EventHandler);
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
                    this.SetLeft(value);
                    break;

                case KnownNti.ReadOnly:
                    ReadOnly = value;
                    break;

                case KnownNti.Value:
                    type = value.Type;
                    Text = ValueHandling.ValueToText(value);
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

                    throw new ErrorException(ErrorCodes.PropertyIsNotFound);
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

                case KnownNti.ReadOnly:
                    return new Variant(ReadOnly);

                case KnownNti.Top:
                    return new Variant(Top, 10);

                case KnownNti.Width:
                    return new Variant(Width, 10);

                case KnownNti.Height:
                    return new Variant(Height, 10);

                case KnownNti.TabIndex:
                    return new Variant(TabIndex, 10);

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
    }
}
