using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using Guineu.Expression;

namespace Guineu.Gui.Compact
{
	class CompactImage : PictureBox, IControl, IGridHosted
	{
		Boolean readOnly;

		// TODO: Use ImageHandling class
		String curPicture;
		Int32 pemStretch;
		String curPictureVal;

		private void ShowPicture()
		{
			String picture = curPicture;
			if (String.IsNullOrEmpty(curPictureVal))
				if (String.IsNullOrEmpty(picture))
					Image = null;
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
			Image = img;
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

		// TODO: PictureVal can be an object (IPicture)
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

				case KnownNti.Picture:
					curPicture = value;
					ShowPicture();
					break;

				case KnownNti.PictureVal:
					curPictureVal = value;
					ShowPicture();
					break;

				case KnownNti.ReadOnly:
					readOnly = value;
					break;

					case KnownNti.Stretch:
					pemStretch = value;
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
					return new Variant(ScaleDown(Left), 10);

				case KnownNti.Top:
					return new Variant(ScaleDown(Top), 10);

				case KnownNti.Width:
					return new Variant(ScaleDown(Width), 10);

				case KnownNti.Height:
					return new Variant(ScaleDown(Height), 10);

				case KnownNti.Picture:
					return new Variant(curPicture);

				case KnownNti.PictureVal:
					return new Variant(curPictureVal);

				case KnownNti.ReadOnly:
					return new Variant(readOnly);

				case KnownNti.Stretch:
					return new Variant(pemStretch, 10);

				case KnownNti.TabIndex:
					return new Variant(TabIndex, 10);

				case KnownNti.Value:
					return GetValue();

				case KnownNti.Visible:
					return new Variant(Visible);

				default:
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

				case KnownNti.Click:
					this.RaiseEvent(EventHandler, KnownNti.Click, parms);
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
