namespace Guineu.ObjectEngine
{
	class StringProperty : PropertyMember
	{
		protected override void DoValidateValue(Variant val)
		{
			if (val.Type != VariantType.Character)
			{
				throw new ErrorException(ErrorCodes.DataTypeInvalid);
			}
		}
	}
}
