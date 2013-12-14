using System;

namespace Guineu.Gui.Desktop
{
	class ValueHandling
	{

		public static String ValueToText(Variant value)
		{
			var type = value.Type;
			return value;
		}
		public static Variant TextToValue(String text, VariantType type)
		{
            switch (type)
            {
                //case VariantType.Integer:
                //  break;
                //case VariantType.Logical:
                //  break;
                case VariantType.Character:
                    return new Variant(text);
                //case VariantType.Number:
                //  break;
                //case VariantType.Object:
                //  break;
                case VariantType.Date:
                    DateTime dt;
                    try
                    {
                        dt = DateTime.Parse(text);
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
                    return new Variant(text);
            }
		}
	}
}
