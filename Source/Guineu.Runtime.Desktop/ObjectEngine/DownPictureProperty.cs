using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	partial class DownPictureProperty : StringProperty
	{
		String _Picture;
		UiControl _Owner;

		public DownPictureProperty(String picture)
		{
			_Picture = picture;
			Nti = KnownNti.DownPicture;
		}

		public void AssignParent(UiControl owner)
		{
			_Owner = owner;
			SetString(_Picture);
		}

		public override Variant Get()
		{
			return new Variant(((IGuiButton)_Owner.View).GuiDownPicture);
		}

		public override void Set(Variant value)
		{
			((IGuiButton)_Owner.View).GuiDownPicture = value;
		}
	}
}
