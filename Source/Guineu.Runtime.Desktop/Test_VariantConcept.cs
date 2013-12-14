using System;
using System.Text;
using Guineu;

interface IVariantValue
{
	
}

public struct NewVariant
{
	// readonly VariantType type;
	IVariantValue val;

	public NewVariant(Int32 value, Int32 len)
	{
		// type = VariantType.Integer;
		val = new VariantValueInt32(value);
	}

	public NewVariant(String value)
	{
		// type = VariantType.Character;
		val = new VariantValueString(value);
	}

	public static NewVariant CreateInt32(Int32 value)
	{
		return new NewVariant(value, 10);
	}

	public static implicit operator Int32(NewVariant value)
{
	if(value.val is VariantValueInt32)
			return ((VariantValueInt32)value.val).value;

			return 0;
	}
	//switch (value.type)
	//{
	//  case VariantType.Integer:
	//    return ((VariantValueInt32)value.val).value;

	//  default:
	//    return 0;
	//}

}

class VariantValueInt32 : IVariantValue
{
	readonly internal Int32 value;
	public VariantValueInt32(Int32 val)
	{
		value = val;
	}
}

struct VariantValueString : IVariantValue
{
	readonly StringBuilder value;
	public VariantValueString(String val)
	{
		value = new StringBuilder(val);
		;
	}
}