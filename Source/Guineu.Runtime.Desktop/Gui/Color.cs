using System;
namespace Guineu.Gui
{
    public partial struct Color
    {
        readonly byte red;
        readonly byte green;
        readonly byte blue;
        
        public Color(byte redColor, byte greenColor, byte blueColor)
        {
            red = redColor;
            green = greenColor;
            blue = blueColor;
        }

			public Color(Int32 rgb)
			{
				red = (byte)(rgb & 0x000000FF);
				green =(byte) ((rgb & 0x0000FF00) >> 8);
				blue = (byte) ((rgb & 0x00FF0000) >> 16);
			}

        public static Boolean operator!= (Color c1, Color c2)
        {
            return !Equals(c1, c2);
        }

        public static Boolean operator== (Color c1, Color c2)
        {
            return Equals(c1, c2);
        }

			public static explicit operator Int32(Color c)
			{
				return c.red + c.green * 256 + c.blue * 256 * 256;
			}

        public static Boolean Equals(Color c1, Color c2)
        {
            return (c1.red == c2.red && c1.blue == c2.blue && c1.green == c2.green);
        }

        public override bool Equals(object obj)
        {
            return Equals(this, (Color) obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
