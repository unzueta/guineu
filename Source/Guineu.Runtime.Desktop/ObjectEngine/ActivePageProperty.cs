using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	class ActivePageProperty : PropertyMember
	{
		Int32 initialValue;
		UiControl _Owner;

		IGuiPageframe pgf
		{
			get
			{
				if (_Owner.View is IGuiPageframe)
					return (IGuiPageframe)_Owner.View;
				else
					throw new ErrorException(ErrorCodes.ObjectClassInvalid);
			}
		}

		public ActivePageProperty(Int32 value)
		{
			initialValue = value;
			Nti = KnownNti.ActivePage;
		}

		public void AssignParent(UiControl owner)
		{
			_Owner = owner;
			pgf.GuiActivePage = initialValue;
		}

		public override Variant Get()
		{
			return new Variant(pgf.GuiActivePage, 10);
		}

		public override void Set(Variant value)
		{
			pgf.GuiActivePage = value;
		}
	}

}
