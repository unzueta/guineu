using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
	/// <summary>
	/// ADIR()
	/// </summary>
	partial class ADIR : ExpressionBase
	{

		private static void ChangeArraySize(ArrayMember arr, Int32 length)
		{
			arr.Dimension(length, 5);
		}
        private string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

		}

}