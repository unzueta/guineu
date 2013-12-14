using System;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	class PageCountProperty : PropertyMember
	{
		readonly Int32 pageCount;
		UiControl owner;

		public PageCountProperty(Int32 value)
		{
			pageCount = value;
			Nti = KnownNti.PageCount;
		}

		public void AssignParent(UiControl ctrl)
		{
			owner = ctrl;
			ctrl.ControlCount = pageCount;
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

	
	class PageCountPropertyTemplate : PropertyMember
	{
		readonly Int32 pageCount;
		UiControlTemplate owner;

		public PageCountPropertyTemplate(Int32 value)
		{
			pageCount = value;
			Nti = KnownNti.PageCount;
		}

		public void AssignParent(UiControlTemplate ctrl)
		{
			owner = ctrl;
			ctrl.ControlCount = pageCount;
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
			var newProp = new PageCountPropertyTemplate(pageCount);
			return newProp;
		}
	}

}
