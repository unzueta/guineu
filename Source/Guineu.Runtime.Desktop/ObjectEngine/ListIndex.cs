using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class ListIndexProperty : PropertyMember
	{
		UiControl _Owner;
		Int32 initialValue;

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

		public ListIndexProperty(Int32 value)
		{
			initialValue = value;
			Nti = KnownNti.ListIndex;
		}

		public void AssignParent(UiControl owner)
		{
			_Owner = owner;
			lst.GuiListIndex = initialValue;
		}

		public override Variant Get()
		{
			return new Variant(lst.GuiListIndex, 10);
		}

		public override void Set(Variant value)
		{
			lst.GuiListIndex = value;
		}
	}

}
