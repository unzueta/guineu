using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using Guineu.Expression;
using System.Runtime.InteropServices;

namespace Guineu
{
	partial class FONTMETRIC : ExpressionBase
	{
		[DllImport("Gdi32.dll", CharSet = CharSet.Unicode)]
		static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		[DllImport("Gdi32.dll", CharSet = CharSet.Unicode)]
		static extern bool GetTextMetrics(IntPtr hdc, out TEXTMETRIC lptm);

		[DllImport("Gdi32.dll", CharSet = CharSet.Unicode)]
		static extern bool DeleteObject(IntPtr hdc);

		[StructLayout(LayoutKind.Sequential)]
		internal struct TEXTMETRIC
		{
			public Int32 Height;
			public Int32 Ascent;
			public Int32 Descent;
			public Int32 InternalLeading;
			public Int32 ExternalLeading;
			public Int32 AveCharWidth;
			public Int32 MaxCharWidth;
			public Int32 Weight;
			public Int32 Overhang;
			public Int32 DigitizedAspectX;
			public Int32 DigitizedAspectY;

			public char FirstChar;
			public char LastChar;
			public char DefaultChar;
			public char BreakChar;

			public byte Italic;
			public byte Underlined;
			public byte StruckOut;
			public byte PitchAndFamily;
			public byte CharSet;
		}

		#region compact
		internal TEXTMETRIC GetTextMetrics(Graphics graphics, Font font)
		{
			IntPtr hDC = graphics.GetHdc();
			TEXTMETRIC textMetric;
			IntPtr hFont = font.ToHfont();
			try
			{
				IntPtr hFontPreviouse = SelectObject(hDC, hFont);
				bool result = GetTextMetrics(hDC, out textMetric);
				SelectObject(hDC, hFontPreviouse);
			}
			finally
			{
				DeleteObject(hFont);
				graphics.ReleaseHdc(hDC);
			}
			return textMetric;
		} 
		#endregion

		#region tools
		internal System.Windows.Forms.Form GetForm()
		{
			System.Windows.Forms.TextBox _txt = new System.Windows.Forms.TextBox();
			if (_form == null)
			{
				_form = new System.Windows.Forms.Form();
				_form.Visible = false;
				_form.SuspendLayout();
				_form.Controls.Add(_txt);
			}
			return _form;
		}

		//internal double GetPixelsFromEm(double em)
		//{
		//    return em * 16.0 / 2048.0;
		//}

		internal Graphics getGraphics(Font font)
		{
			System.Drawing.Font toUse = font;
			try
			{
				if (_graphics == null)
				{
					if (toUse == null)
					{
						if (_font == null)
							toUse = getFont("MS Sans Serif", 10, "N");
						else
							toUse = _font;
					}
					_form = GetForm();
					_form.Font = toUse;
					_graphics = _form.CreateGraphics();
				}
				return _graphics;
			}
			catch (Exception)
			{
				return null;
			}
		}
		internal System.Drawing.Font getFont(string cFontName, int nFontSize, string cFontStyle)
		{
			if (_font == null)
			{
				try
				{
					string cfs = cFontStyle.ToUpper();
					FontFamily fm =  FontFamily.GenericSansSerif;

					bool Italic = cfs.LastIndexOf('I') >= 0;
					bool Bold = cfs.LastIndexOf('B') >= 0;
					bool Strikeout = cfs.LastIndexOf('-') >= 0;
					bool Underline = cfs.LastIndexOf('U') >= 0;

					FontStyle fs=FontStyle.Regular; // 0

					if (Italic)
						fs |= FontStyle.Italic;
					if (Bold)
						fs |= FontStyle.Bold;
					if (Strikeout)
						fs |= FontStyle.Strikeout;
					if (Underline)
						fs |= FontStyle.Underline;

					_font = new System.Drawing.Font(fm, nFontSize, fs);
				}
				catch (Exception)
				{
					_font = null;
				}
			}

			return _font;
		}
		internal void resetGraphics()
		{
		}
		#endregion tools

		#region internal
		internal double Italic(Font font, string cfs)
		{
			if (cfs.ToUpper().LastIndexOf("I")>=0)
				return 255.0;
			return 0;
		}
		internal double Underlined(Font font, string cfs)
		{
			if (cfs.ToUpper().LastIndexOf("U") >= 0)
				return 255.0;
			return 0;
		}
		internal double Strikeout(Font font, string cfs)
		{
			if (cfs.LastIndexOf("-") >= 0)
				return 255.0;
			return 0;
		}
		internal double FirstChar(Font font)
		{	// to use : VisualStyleRenderer.GetTextMetrics
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			return (double)tm.FirstChar;
		}
		internal double LastChar(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			return (double)tm.LastChar;
		}
		internal double DefaultChar(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			return (double)tm.DefaultChar;
		}
		internal double WordBreak(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			return (double)tm.BreakChar;
		}
		internal double PitchAndFamilly(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			return (double)tm.PitchAndFamily;
		}
		internal double CharacterSet(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			return (double)tm.CharSet;
		}
		internal double Overhang(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			return (double)tm.Overhang;
		}
		internal double HorizontalAspect(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			return (double)tm.DigitizedAspectX;
		}
		internal double VerticalAspect(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			return (double)tm.DigitizedAspectY;
		}
		internal double CharacterHeight(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			double retVal = 1.0 * tm.Height;

			return retVal;
		}
		internal double CharacterAscent(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			double retVal = 1.0 * tm.Ascent;
			return retVal;
		}
		internal double CharacterDescent(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			double retVal = 1.0 * tm.Descent;
			return retVal;
		}
		internal double Leading(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			double retVal = 1.0 * tm.InternalLeading;
			return retVal;
		}
		internal double ExtraLeading(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			double retVal = 1.0 * tm.ExternalLeading;
			return retVal;
		}
		internal double AverageWidth(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			double retVal = 1.0 * tm.AveCharWidth;
			return retVal;
		}
		internal double MaximumWidth(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			double retVal = 1.0 * tm.MaxCharWidth;
			return retVal;
		}
		internal double FontWeight(Font font)
		{
			Graphics g = getGraphics(font);
			TEXTMETRIC tm = GetTextMetrics(g, font);
			double retVal = 1.0 * tm.Weight;
			return retVal;
		}

		#endregion internal

		#region internal todo
		#endregion Internal todo

	}

}