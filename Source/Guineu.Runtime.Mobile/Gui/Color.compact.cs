namespace Guineu.Gui
{
    public partial struct Color
    {
			// (...) create constructor for Color.

        public static implicit operator System.Drawing.Color(Color c)
        {
            System.Drawing.Color clr = System.Drawing.Color.FromArgb(c.red, c.green, c.blue);
            return clr;
        }

        public static implicit operator Color(System.Drawing.Color clr)
        {
					return new Color(clr.R, clr.G,clr.B);
        }
    }
}
