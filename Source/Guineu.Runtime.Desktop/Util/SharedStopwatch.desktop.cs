using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Guineu.Util
{
	class SharedStopwatch
	{
		Stopwatch sw;

		public SharedStopwatch()
		{
			sw = new Stopwatch();
		}

		public void Start()
		{
			sw.Start();
		}

		public void Stop()
		{
			sw.Stop();
		}

		public void Reset()
		{
			sw.Reset();
		}

		public long ElapsedTicks
		{
			get { return sw.ElapsedTicks; }
		}

		static public long Frequency
		{
			get { return Stopwatch.Frequency; }
		}
	}
}
