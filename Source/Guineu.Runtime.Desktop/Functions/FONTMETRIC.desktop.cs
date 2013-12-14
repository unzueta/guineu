using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using Guineu.Expression;
using System.Windows.Forms.VisualStyles;

namespace Guineu
{
	partial class FONTMETRIC : ExpressionBase
	{
		VisualStyleRenderer _vr = null;

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
			Font toUse = font;
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
		internal Font getFont(string cFontName, int nFontSize, string cFontStyle)
		{
			if (_font == null)
			{
				try
				{
					string cfs = cFontStyle.ToUpper();
					FontFamily fm = new FontFamily(cFontName);

					bool Italic = cfs.LastIndexOf('I') >= 0;
					bool Bold = cfs.LastIndexOf('B') >= 0;
					bool Strikeout = cfs.LastIndexOf('-') >= 0;
					bool Underline = cfs.LastIndexOf('U') >= 0;

					FontStyle fs = FontStyle.Regular; // 0

					if (Italic)
						fs |= FontStyle.Italic;
					if (Bold)
						fs |= FontStyle.Bold;
					if (Strikeout)
						fs |= FontStyle.Strikeout;
					if (Underline)
						fs |= FontStyle.Underline;

					_font = new Font(fm, nFontSize, fs);
				}
				catch (Exception)
				{
					_font = null;
				}
			}

			return _font;
		}
		internal VisualStyleRenderer GetRenderer()
		{
			if (_vr == null)
			{
				VisualStyleElement ve = VisualStyleElement.TextBox.TextEdit.Normal;
				_vr = new VisualStyleRenderer(ve);
			}
			return _vr;
		}
		#endregion tools

		#region internal
		internal double Italic(Font font, string cfs)
		{
			if (font.Italic)
				return 255.0;
			return 0;
		}
		internal static double Underlined(Font font, string cfs)
		{
			if (font.Underline)
				return 255.0;
			return 0;
		}
		internal static double Strikeout(Font font, string cfs)
		{
			if (font.Strikeout)
				return 255.0;
			return 0;
		}
		internal double FirstChar(Font font)
		{	// to use : VisualStyleRenderer.GetTextMetrics
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			return (double)tm.FirstChar;
		}
		internal double LastChar(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			return (double)tm.LastChar;
		}
		internal double DefaultChar(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			return (double)tm.DefaultChar;
		}
		internal double WordBreak(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			return (double)tm.BreakChar;
		}
		internal double PitchAndFamilly(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			return (double)tm.PitchAndFamily;
		}
		internal double CharacterSet(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			return (double)tm.CharSet;
		}
		internal double Overhang(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			return (double)tm.Overhang;
		}
		internal double HorizontalAspect(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			return (double)tm.DigitizedAspectX;
		}
		internal double VerticalAspect(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			return (double)tm.DigitizedAspectY;
		}
		internal double CharacterHeight(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			double retVal = 1.0 * tm.Height;

			return retVal;
		}
		internal double CharacterAscent(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			double retVal = 1.0 * tm.Ascent;
			return retVal;
		}
		internal double CharacterDescent(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			double retVal = 1.0 * tm.Descent;
			return retVal;
		}
		internal double Leading(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			double retVal = 1.0 * tm.InternalLeading;
			return retVal;
		}
		internal double ExtraLeading(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			double retVal = 1.0 * tm.ExternalLeading;
			return retVal;
		}
		internal double AverageWidth(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			double retVal = 1.0 * tm.AverageCharWidth;
			return retVal;
		}
		internal double MaximumWidth(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			double retVal = 1.0 * tm.MaxCharWidth;
			return retVal;
		}
		internal double FontWeight(Font font)
		{
			Graphics g = getGraphics(font);
			VisualStyleRenderer vs = GetRenderer();
			TextMetrics tm = vs.GetTextMetrics(g);
			double retVal = 1.0 * tm.Weight;
			return retVal;
		}

		#endregion internal

		#region internal todo
		#endregion Internal todo

	}
}