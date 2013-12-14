using System;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	class ColumnCountProperty : PropertyMember
	{
		readonly Int32 initialValue;
		UiControl owner;

		public ColumnCountProperty(Int32 value)
		{
			initialValue = value;
			Nti = KnownNti.ColumnCount;
		}

		public void AssignParent(UiControl ctrl)
		{
			owner = ctrl;
			ctrl.ControlCount = initialValue;
		}

		public override Variant Get()
		{
			return new Variant(owner.ControlCount, 10);
		}

		public override void Set(Variant value)
		{
			owner.ControlCount = value;
		}
	}

	class ColumnCountPropertyTemplate : PropertyMember
	{
		readonly Int32 initialValue;
		UiControlTemplate owner;

		public ColumnCountPropertyTemplate(Int32 value)
		{
			initialValue = value;
			Nti = KnownNti.ColumnCount;
		}

		public void AssignParent(UiControlTemplate ctrl)
		{
			owner = ctrl;
			ctrl.ControlCount = initialValue;
		}

		public override Variant Get()
		{
			return new Variant(owner.ControlCount, 10);
		}

		public override void Set(Variant value)
		{
			owner.ControlCount = value;
		}

		internal override Member Clone()
		{
			var newProp = new ColumnCountPropertyTemplate(initialValue);
			return newProp;
		}
	}

}
