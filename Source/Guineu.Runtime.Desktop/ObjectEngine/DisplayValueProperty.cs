using System;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	// (...) Need to implement syntax to assign value by indeyx
	class DisplayValueProperty : PropertyMember
	{
		readonly String initialValue;
		UiControl owner;

		IGuiList Lst
		{
			get
			{
				if (owner.View is IGuiList)
					return (IGuiList)owner.View;
				throw new ErrorException(ErrorCodes.ObjectClassInvalid);
			}
		}

		public DisplayValueProperty()
		{
			Nti = KnownNti.DisplayValue;
		}

		public DisplayValueProperty(String value)
			: this()
		{
			initialValue = value;
		}

		public void AssignParent(UiControl ctrl)
		{
			owner = ctrl;
			if (String.IsNullOrEmpty(initialValue))
				Lst.GuiDisplayValue = "";
			else
				Lst.GuiDisplayValue = initialValue;
		}

		public override void Set(Variant value)
		{
			switch (value.Type)
			{
				case VariantType.Integer:
				case VariantType.Number:
					Lst.GuiListIndex = value;
					break;
				case VariantType.Character:
					Lst.GuiDisplayValue = value;
					break;
				default:
					throw new ErrorException(ErrorCodes.DataTypeInvalid);
			}
		}

		public override Variant Get()
		{
			return new Variant(Lst.GuiDisplayValue);
		}
	}
}
