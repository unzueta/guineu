using System;

namespace Guineu
{
    /// <summary>
    /// ALINES()
    /// </summary>
    partial class ALINES
    {
        private static string[] ChangeArraySize(string[] oldArray, Int32 newSize)
        {
			var newArray = new string[newSize];
			int preserveLength = Math.Min(oldArray.Length, newSize);
			if (preserveLength > 0)
				Array.Copy(oldArray, newArray, preserveLength);
			return newArray;
        }
    }
}
