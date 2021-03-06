using System;
using System.Windows.Forms;
using System.ComponentModel;
using Guineu.Expression;

namespace Guineu.Gui.Desktop
{
    class DesktopEditbox : TextBox, IControl
    {
        public DesktopEditbox()
        {
            ScrollBars = ScrollBars.Vertical;
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);
            this.ValidEvent(this, EventHandler, ValueHandling.TextToValue(Text, VariantType.Character));
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
            lock (this)
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

                    case KnownNti.ReadOnly:
                        ReadOnly = value;
                        break;

                    case KnownNti.TabIndex:
                        TabIndex = value;
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

                    case KnownNti.SelStart:
                        SelectionStart = value;
                        break;

                    case KnownNti.SelLength:
                        SelectionLength = value;
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

                case KnownNti.SelStart:
                    return new Variant(SelectionStart, 10);

                case KnownNti.SelLength:
                    return new Variant(SelectionLength, 10);

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
