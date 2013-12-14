using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class ListCountProperty : PropertyMember
	{
		UiControl _Owner;

		IGuiList lst
		{
			get
			{
				if (_Owner.View is IGuiList)
					return (IGuiList)_Owner.View;
				else
					throw new ErrorException(ErrorCodes.ObjectClassInvalid);
			}
		}

		public ListCountProperty()
		{
			Nti = KnownNti.ListCount;
		}

		public void AssignParent(UiControl owner)
		{
			_Owner = owner;
		}

		public override Variant Get()
		{
			return new Variant(lst.GuiListCount, 10);
		}

		public override void Set(Variant value)
		{
			throw new ErrorException(ErrorCodes.PropertyIsReadOnly);
		}


	}

}
