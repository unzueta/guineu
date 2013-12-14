using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Guineu.Expression;

namespace Guineu.Gui.Compact
{
	class CompactColumn : IControl
	{
		CompactGrid grid;
		Int32 columnIndex;

		public CompactHeader Header { get; private set; }
		public IControl CurrentControl { get; private set; }
		public Int32 Width { get { return width; } }

		// The column object collects data from the model before the view
		// is available. 
		string tag;
		Variant text;
		Int32 width;

		public void SetVariant(KnownNti nti, Variant value)
		{
			switch (nti)
			{
				case KnownNti.BackColor:
				case KnownNti.Enabled:
				case KnownNti.ForeColor:
				case KnownNti.Visible:
				case KnownNti.FontBold:
				case KnownNti.FontItalic:
				case KnownNti.FontName:
				case KnownNti.FontSize:
				case KnownNti.FontUnderline:
				case KnownNti.FontStrikeThru:
				case KnownNti.Left:
				case KnownNti.Top:
				case KnownNti.Height:
					break;

				case KnownNti.Value:
					text = value;
					break;

				case KnownNti.Tag:
					tag = value;
					break;

				case KnownNti.Width:
					width = ScaleUp(value);
					if (grid != null)
					{
						DataGridColumnStyle style = grid.GridColumnStyles(columnIndex);
						style.Width = width;
					}
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
					return new Variant((Int32)new Color(255, 255, 255), 10);

				case KnownNti.Enabled:
					return new Variant(true);

				case KnownNti.ForeColor:
					return new Variant((Int32)new Color(0, 0, 0), 10);

				case KnownNti.Tag:
					return new Variant(tag);

				case KnownNti.Width:
					Int32 w;
					if (grid != null)
						w = grid.GridColumnStyles(columnIndex).Width;
					else
						w = width;
					return new Variant(ScaleDown(w), 10);

				case KnownNti.Visible:
					return new Variant(true);

				case KnownNti.Value:
					return text;

				default:
					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		public Variant CallMethod(KnownNti name, ParameterCollection parms)
		{
			switch (name)
			{
				case KnownNti.AddObject:
					Object obj = parms[0].Get().ToNative();
					if (obj is CompactHeader)
						Header = (obj as CompactHeader);

					else if (CurrentControl == null && obj is IControl)
						CurrentControl = (obj as IControl);

					return new Variant(true);

				case KnownNti.Move:
					return new Variant(true);

				default:
					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		public event Action<EventData> EventHandler;

		internal void RaiseEvent(Nti name)
		{
			this.RaiseEvent(EventHandler, name, new ParameterCollection());
		}

		static Int32 ScaleDown(Int32 hires)
		{
			var mgr = (CompactManager)GuineuInstance.WinMgr;
			return (Int32)Math.Round(hires / mgr.Scale, 0);
		}

		static Int32 ScaleUp(Int32 lores)
		{
			var mgr = (CompactManager)GuineuInstance.WinMgr;
			return (Int32)Math.Round(lores * mgr.Scale, 0);
		}

		internal void LinkToGrid(CompactGrid ctrl, Int32 index)
		{
			grid = ctrl;
			columnIndex = index;
			Debug.Assert(Header != null, "LinkToGrid called before model was created completely.");
			Header.LinkToGrid(grid, columnIndex);
		}
	}

	class DataGridImageColumn : DataGridColumnStyle
	{
		readonly CompactImage img;

		public DataGridImageColumn(CompactImage ctrl)
		{
			img = ctrl;
		}
		protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
		{
			Rectangle rect = bounds;
			g.FillRectangle(backBrush, rect);
			rect.Offset(0, 2);
			rect.Height -= 2;
			g.DrawImage(img.Image, bounds, 0, 0, bounds.Width, bounds.Height, GraphicsUnit.Pixel, new ImageAttributes());
			//g.DrawString("Hi", new Font("Arial", 10, FontStyle.Regular), foreBrush, bounds);
			//g.DrawImage(img.Image, rect,rect,GraphicsUnit.Pixel);
		}
	}

	class DataGridButtonColumn : DataGridColumnStyle
	{
		readonly CompactButton button;

		public DataGridButtonColumn(CompactButton ctrl)
		{
			button = ctrl;
		}
		protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
		{
			Rectangle rect = bounds;
			g.FillRectangle(backBrush, rect);
			rect.Offset(2, 2);
			rect.Height -= 2;
			rect.Width -= 2;
			var format = new StringFormat {LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center};
			g.DrawString(button.Text, button.Font, foreBrush, bounds,format);
		}
	}

}
