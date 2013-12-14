using System;
using System.Windows.Forms;
using Guineu.Expression;

namespace Guineu.Gui.Compact
{
	class CompactHeader : IControl
	{
		String caption;
		internal String Caption { get { return caption; } }

		CompactGrid grid;
		Int32 columnIndex;

		Int32 foreColor;
		Int32 backColor;

		public void SetVariant(KnownNti nti, Variant value)
		{
			switch (nti)
			{
				case KnownNti.Caption:
					caption = value;
					if (grid != null)
					{
						DataGridColumnStyle style = grid.GridColumnStyles(columnIndex);
						style.HeaderText = caption;
					}
					break;

				case KnownNti.BackColor:
					if (grid == null)
						backColor = value;
					else
						grid.HeaderBackColor = new Color(value);
					break;

				case KnownNti.Enabled:
				case KnownNti.ForeColor:
					if(grid == null)
						foreColor = value;
					else
					grid.HeaderForeColor = new Color(value);
					break;

				case KnownNti.FontBold:
				case KnownNti.FontItalic:
				case KnownNti.FontName:
				case KnownNti.FontSize:
				case KnownNti.FontUnderline:
				case KnownNti.FontStrikeThru:
				case KnownNti.Left:
				case KnownNti.Picture:
				case KnownNti.Top:
				case KnownNti.Height:
				case KnownNti.Visible:
				case KnownNti.Width:
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
					if(grid == null)
						return new Variant(backColor, 10);
					return new Variant((Int32) (Color)grid.HeaderBackColor, 10);

				case KnownNti.Caption:
					if (grid != null)
						return new Variant(grid.GridColumnStyles(columnIndex).HeaderText);
					return new Variant(caption);

				case KnownNti.ForeColor:
					if (grid == null)
						return new Variant(foreColor, 10);
					return new Variant((Int32)(Color)grid.HeaderForeColor, 10);

				default:
					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		public Variant CallMethod(KnownNti name, ParameterCollection parms)
		{
			switch (name)
			{
				case KnownNti.Move:
					return new Variant(true);
					
				default:
					throw new ErrorException(ErrorCodes.PropertyIsNotFound);
			}
		}

		public event Action<EventData> EventHandler;

		internal void LinkToGrid(CompactGrid ctrl, Int32 index)
		{
			grid = ctrl;
			columnIndex = index;
			grid.HeaderBackColor = new Color(backColor);
			grid.HeaderForeColor = new Color(foreColor);
		}
	}
}
