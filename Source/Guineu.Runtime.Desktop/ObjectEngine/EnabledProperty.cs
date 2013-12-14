using System;
using Guineu.Expression;

namespace Guineu.ObjectEngine
{
	class EnabledProperty : PropertyMember
	{
		readonly Boolean enabled;
		UiControl ownerControl;

		public EnabledProperty(Boolean enabled)
		{
			this.enabled = enabled;
			Nti = KnownNti.Enabled;
		}

		public void AssignParent(UiControl owner)
		{
			ownerControl = owner;
			Set(new Variant(enabled));
		}

		public override Variant Get()
		{
			return ownerControl.View.GetVariant(KnownNti.Enabled);
		}

		public override void Set(Variant value)
		{
			ownerControl.View.SetVariant(KnownNti.Enabled, value);
		}

	}

}
