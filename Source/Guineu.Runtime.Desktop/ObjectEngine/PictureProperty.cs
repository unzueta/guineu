using System.IO;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	class PictureProperty : GenericProperty
	{
		public PictureProperty(Variant value) : base(KnownNti.Picture, value, null, VariantType.Character) { }

		public override void Set(Variant value)
		{
			try
			{
				base.Set(value);
			}
			catch (DirectoryNotFoundException)
			{
				base.Set(new Variant(""));
			}
			catch (FileNotFoundException)
			{
				base.Set(new Variant(""));
			}
		}
	}
}
