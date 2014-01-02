using System;
using System.Drawing;
using System.Windows.Forms; 
using Guineu.Expression;

namespace Guineu.Gui.Desktop
{
    // TODO: extract interface for property extensions
    // TODO: replace switch by HashSet
    static class FontHandling
    {
        public static Boolean Handles(KnownNti nti)
        {
            return nti == KnownNti.FontBold || nti == KnownNti.FontItalic || nti == KnownNti.FontName ||
                   nti == KnownNti.FontSize || nti == KnownNti.FontStrikeThru || nti == KnownNti.FontUnderline;
        }

        public static void Set(Control ctrl, KnownNti nti, Variant value)
        {
            switch (nti)
            {
                case KnownNti.FontBold:
                    SetFontStyle(FontStyle.Bold, ctrl, value);
                    break;

                case KnownNti.FontItalic:
                    SetFontStyle(FontStyle.Italic, ctrl, value);
                    break;

                case KnownNti.FontName:
                    ctrl.Font = new Font(value.ToString(), ctrl.Font.Size, ctrl.Font.Style, ctrl.Font.Unit);
                    break;

                case KnownNti.FontSize:
                    ctrl.Font = new Font(ctrl.Font.Name, value, ctrl.Font.Style, ctrl.Font.Unit);
                    break;

                case KnownNti.FontStrikeThru:
                    SetFontStyle(FontStyle.Strikeout, ctrl, value);
                    break;

                case KnownNti.FontUnderline:
                    SetFontStyle(FontStyle.Underline, ctrl, value);
                    break;
            }
        }

        public static Variant Get(Control ctrl, KnownNti nti)
        {
            switch (nti)
            {
                case KnownNti.FontBold:
                    return new Variant(ctrl.Font.Bold);

                case KnownNti.FontItalic:
                    return new Variant(ctrl.Font.Italic);

                case KnownNti.FontName:
                    return new Variant(ctrl.Font.Name);

                case KnownNti.FontSize:
                    return new Variant((Int32)ctrl.Font.Size, 10);

                case KnownNti.FontStrikeThru:
                    return new Variant(ctrl.Font.Strikeout);

                case KnownNti.FontUnderline:
                    return new Variant(ctrl.Font.Underline);

            }
            throw new ErrorException(ErrorCodes.PropertyIsNotFound, nti);
        }

        private static void SetFontStyle(FontStyle style, Control ctrl, bool setStyle)
        {
            FontStyle newStyle;
            if (setStyle)
                newStyle = ctrl.Font.Style | style;
            else
                newStyle = ctrl.Font.Style & ~style;
            ctrl.Font = new Font(ctrl.Font, newStyle);
        }
    }
}
