using System;
using System.Drawing;
using System.Windows.Forms;
using OpenNETCF.Windows.Forms;
using System.IO;
using Guineu.Expression;
using System.Drawing.Imaging;

namespace Guineu.Gui.Compact
{
    class CompactSignature : Signature, IControl
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

        public void SetVariant(KnownNti nti, Variant value)
        {
            switch (nti)
            {
                case KnownNti.BackColor:
                    BackColor = new Color(value);
                    break;

                case KnownNti.BorderColor:
                    BorderColor = new Color(value);
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

                case KnownNti.Value:
                    Byte[] b = value.GetBytes();
                    if (b.Length == 0)
                        Clear();
                    else
                        LoadSignatureEx(b);
                    break;

                case KnownNti.Visible:
                    Visible = value;
                    break;

                case KnownNti.PenWidth:
                    PenWidth = value;
                    break;

								case KnownNti.Picture:
										curPicture = value;
										ShowPicture();
										break;

								case KnownNti.PictureVal:
										curPictureVal = value;
										ShowPicture();
										break;

                case KnownNti.BorderStyle:
                    switch ((Int32)value)
                    {
                        case 0:
                            BorderStyle = BorderStyle.None;
                            break;
                        case 1:
                            BorderStyle = BorderStyle.FixedSingle;
                            break;
                        case 2:
                            BorderStyle = BorderStyle.Fixed3D;
                            break;
                    }
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

                case KnownNti.BorderColor:
                    return new Variant((Int32)(Color)BorderColor, 10);

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

                case KnownNti.TabIndex:
                    return new Variant(TabIndex, 10);

                case KnownNti.Visible:
                    return new Variant(Visible);

								case KnownNti.Picture:
										return new Variant(curPicture);

								case KnownNti.PictureVal:
										return new Variant(curPictureVal);

				case KnownNti.Value:
					return new Variant(GetSignatureEx());

								case KnownNti.Signature:
                    using (var s = new MemoryStream())
                    {
                        ToBitmap().Save(s, ImageFormat.Bmp);
                        return new Variant(s.ToArray());
                    }

                case KnownNti.PenWidth:
                    return new Variant(PenWidth, 10, 2);

                case KnownNti.BorderStyle:
                    switch (BorderStyle)
                    {
                        case BorderStyle.Fixed3D:
                            return new Variant(2, 10);
                        case BorderStyle.FixedSingle:
                            return new Variant(1, 10);
                        case BorderStyle.None:
                            return new Variant(0, 10);
                    }
                    throw new ErrorException(ErrorCodes.InternalConsistency);

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

				String curPicture;
				String curPictureVal;

				private void ShowPicture()
				{
					String picture = curPicture;
					if (String.IsNullOrEmpty(curPictureVal))
						if (String.IsNullOrEmpty(picture))
							BackgroundImage = null;
						else
						{
							Stream s = GuineuInstance.FileMgr.Open(
								picture,
								FileMode.Open,
								FileAccess.Read,
								FileShare.Read
							);
							using (s)
								LoadImage(s);
						}
					else
						using (Stream s = new MemoryStream(GuineuInstance.CurrentCp.GetBytes(curPictureVal)))
							LoadImage(s);
				}

				private void LoadImage(Stream s)
				{
					var img = new Bitmap(s);
					Size = img.Size;
					BackgroundImage = img;
				}
		}
}