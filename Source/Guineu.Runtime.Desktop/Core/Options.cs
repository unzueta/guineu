using System;

using System.Collections.Generic;
using System.Text;

namespace Guineu.Core
{
	class Options
	{
		/// <summary>
		/// Click event executes manually when the user presses Enter. This should be the default
		/// behavior. However, on some devices it seems that Enter does nothing at all.
		/// </summary>
		public Boolean ButtonClickOnEnter { get; set; }
	}
}
