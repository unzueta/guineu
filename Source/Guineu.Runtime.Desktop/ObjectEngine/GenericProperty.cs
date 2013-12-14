using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	public class GenericProperty : PropertyMember
	{
		readonly Variant initialValue;
		IControl control;
		readonly VariantType supportedType;

		public GenericProperty(KnownNti name, Variant value, IControl owner = null, VariantType isType=VariantType.Unknown)
		{
			initialValue = value;
			Nti = name;
			control = owner;
			supportedType = isType;
		}

		public void AssignParent(IControl owner)
		{
			control = owner;
			control.SetVariant(Nti.ToKnownNti(), initialValue);
		}

		public override Variant Get()
		{
			return control.GetVariant(Nti.ToKnownNti());
		}

		public override void Set(Variant val)
		{
			control.SetVariant(Nti.ToKnownNti(), val);
		}
		protected override void DoValidateValue(Variant val)
		{
			if (supportedType == VariantType.Unknown)
				return;

			if (val.Type != supportedType)
				throw new ErrorException(ErrorCodes.DataTypeInvalid);
		}
	}

}
