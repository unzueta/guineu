using System;
using System.Windows.Forms;
using Guineu.Expression;

namespace Guineu.Gui.Desktop
{
    class DesktopPageframe : TabControl, IControl, IGuiPageframe
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

        Int32 lastIndex;

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            if (SelectedIndex >= 0)
            {
                var pg = (DesktopTabPage)TabPages[SelectedIndex];
                if (pg.Enabled)
                {
                    if (Visible)
                        pg.RaiseActivate(e);
                }
                else
                    SelectedIndex = lastIndex;
            }
            lastIndex = SelectedIndex;
        }
        public int GuiActivePage
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

                case KnownNti.Visible:
                    return new Variant(Visible);

                default:
                    throw new ErrorException(ErrorCodes.PropertyIsNotFound, nti);
            }
        }

        public Variant CallMethod(KnownNti name, ParameterCollection parms)
        {
            switch (name)
            {
                case KnownNti.AddObject:
                    this.AddControl(parms);
                    return new Variant(true);

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
    }
}
