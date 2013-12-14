using System;
using System.IO;

namespace Guineu
{
	partial class CLEAR : ICommand
	{
		partial void DoClear()
		{
			Console.Clear();
		}
	}
}
