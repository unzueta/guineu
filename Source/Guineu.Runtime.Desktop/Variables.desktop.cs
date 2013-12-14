using System;

namespace Guineu
{

	//=====================================================================================
	partial class ArrayMember : ValueMember
	{
		internal void Dimension(Int64 d1)
		{
			SetDimension(d1);
			Array.Resize<ValueMember>(ref val, (Int32)d1);
		}

		internal void Dimension(Int64 d1, Int64 d2)
		{
			SetDimension(d1, d2);
			Array.Resize<ValueMember>(ref val, (Int32)(d1 * d2));
		}
	}
}