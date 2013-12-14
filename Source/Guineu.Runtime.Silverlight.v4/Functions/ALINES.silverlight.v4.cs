using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
    /// <summary>
    /// ALINES()
    /// </summary>
    partial class ALINES : ExpressionBase
    {
        private static string[] ChangeArraySize(string[] oldArray, Int32 newSize)
        {
			string[] newArray = new string[newSize];
			int preserveLength = System.Math.Min(oldArray.Length, newSize);
			if (preserveLength > 0)
				System.Array.Copy(oldArray, newArray, preserveLength);
			return newArray;
        }
    }
}
