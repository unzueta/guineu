using System;
using System.Collections.Generic;
using System.Text;

namespace Guineu.Data
{
	public class SptConnection
	{
		private ISptEngine _Engine;

		public ISptEngine Engine
		{
			get { return _Engine; }
			set { _Engine = value; }
		}
	}
}
