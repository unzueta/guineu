using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	partial class FONTMETRIC : ExpressionBase, IDisposable
	{
		ExpressionBase m_nAttribute;
		ExpressionBase m_cFontName;
		ExpressionBase m_nFontSize;
		ExpressionBase m_cFontStyle;

		Graphics _graphics;
		System.Drawing.Font _font;
		System.Windows.Forms.Form _form;


		override internal void Compile(Compiler comp)
		{
			List<ExpressionBase> param = comp.GetParameterList();
			switch (param.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 1:
					m_nAttribute = param[0];
					break;
				case 2:
					throw new ErrorException(ErrorCodes.TooFewArguments);
				case 3:
					m_nAttribute = param[0];
					m_cFontName = param[1];
					m_nFontSize = param[2];
					break;
				case 4:
					m_nAttribute = param[0];
					m_cFontName = param[1];
					m_nFontSize = param[2];
					m_cFontStyle = param[3];
					break;
				default:
					throw new ErrorException(ErrorCodes.TooManyArguments);
			}
			FixedInt = true;
		}

		internal bool CheckParams(CallingContext context)
		{
			Variant p;
			p = m_nAttribute.GetVariant(context);
			if (p.Type != VariantType.Integer && p.Type != VariantType.Number)
				return false;

			if (m_cFontName != null)
			{
				if (m_cFontName.CheckString(context, false))
					return false;
			}

			if (m_nFontSize != null)
			{
				if (m_cFontStyle == null)
					return false;

				p = m_nFontSize.GetVariant(context);
				if (p.Type != VariantType.Integer && p.Type != VariantType.Number)
					return false;
			}

			if (m_cFontStyle != null)
			{
				if (m_cFontStyle.CheckString(context, false))
					return false;
			}

			return true;
		}

		override internal Variant GetVariant(CallingContext context)
		{
			double retVal = 0;

			int nAttribute;
			string cFontName;
			int nFontSize;
			string cFontStyle;
			Font fFont;

			if (CheckParams(context))
			{
				nAttribute=m_nAttribute.GetInt(context);

				if (m_cFontName != null)
					cFontName = m_cFontName.GetString(context);
				else
					cFontName = "Microsoft Sans Serif";

				if (m_nFontSize != null)
					nFontSize = m_nFontSize.GetInt(context);
				else
					nFontSize = 10;

				if (m_cFontStyle != null)
					cFontStyle = m_cFontStyle.GetString(context);
				else
					cFontStyle = "N";

				fFont = getFont(cFontName, nFontSize, cFontStyle);
				if(fFont==null)
					throw new ErrorException(ErrorCodes.Syntax, "getFont unable to create font" + cFontName.ToString() + "-" + nFontSize.ToString() + "-"+cFontStyle.ToString());

				switch (nAttribute)
				{
					case 1:	//Character height in pixels
						retVal = CharacterHeight(fFont);
						break;
					case 2:	//Character ascent (units above baseline) in pixels
						retVal = CharacterAscent(fFont);
						break;
					case 3:	//Character descent (units below baseline) in pixels
						retVal = CharacterDescent(fFont);
						break;
					case 4:	//Leading (space between lines) in pixels
						retVal = Leading(fFont);
						break;
					case 5:	//Extra leading in pixels
						retVal = ExtraLeading(fFont);
						break;
					case 6:	//Average character width in pixels
						retVal = AverageWidth(fFont);
						break;
					case 7:	//Maximum character width in pixels
						retVal = MaximumWidth(fFont);
						break;
					case 8:	//Font weight.
						retVal = FontWeight(fFont);
						break;
					case 9:	//Italic (0 = no, nonzero = yes)
						retVal = Italic(fFont, cFontStyle);
						break;
					case 10:	//Underlined (0 = no, nonzero = yes)
						retVal = Underlined(fFont, cFontStyle);
						break;
					case 11:	//Strikeout (0 = no, nonzero = yes)
						retVal = Strikeout(fFont, cFontStyle);
						break;
					case 12:	//First character defined in font
						retVal = FirstChar(fFont);
						break;
					case 13:	//Last character defined in font
						retVal = LastChar(fFont);
						break;
					case 14:	//Default character (substituted for characters not in font)
						retVal = DefaultChar(fFont);
						break;
					case 15:	//Word-break character
						retVal = WordBreak(fFont);
						break;
					case 16:	//Pitch and family
						retVal = PitchAndFamilly(fFont);
						break;
					case 17:	//Character set
						retVal = CharacterSet(fFont);
						break;
					case 18:	//Overhang (extra added width)
						retVal = Overhang(fFont);
						break;
					case 19:	//Horizontal aspect for font device
						retVal = HorizontalAspect(fFont);
						break;
					case 20:	//Vertical aspect for font device
						retVal = VerticalAspect(fFont);
						break;
					default:	//unknown
						throw new ErrorException(ErrorCodes.InvalidArgument);
				}
			}
			else
				throw new ErrorException(ErrorCodes.InvalidArgument);

			return new Variant(retVal, 13, 0);
		}

		internal override int GetInt(CallingContext context)
		{
			return GetVariant(context);
		}

		internal override double GetDouble(CallingContext context)
		{
			return GetVariant(context);
		}

		#region IDisposable Members

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_font.Dispose();
				_form.Close();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		#endregion
	}

}