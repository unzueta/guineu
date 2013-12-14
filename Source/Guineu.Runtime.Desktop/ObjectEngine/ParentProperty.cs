using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	partial class ParentProperty : PropertyMember
	{
		NestedClass _Owner;

		public ParentProperty()
		{
			Nti = KnownNti.Parent;
		}

		public void AssignParent(NestedClass owner)
		{
			_Owner = owner;
		}

		public override Variant Get()
		{
			return new Variant(_Owner.Parent);
		}

		public override void Set(Variant val)
		{
			throw new ErrorException(ErrorCodes.PropertyIsReadOnly, Nti.ToString());
		}
	}
}
