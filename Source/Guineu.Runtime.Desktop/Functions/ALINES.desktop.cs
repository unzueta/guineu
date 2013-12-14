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
            Array.Resize(ref oldArray, newSize);
            return oldArray;
        }

    }
}
