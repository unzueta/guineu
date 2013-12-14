using System;

namespace Guineu.Functions
{
    partial class SECONDS
    {
        public class TickProvider
        {
            internal uint Ticks
            {
                get { return (uint)Environment.TickCount; }
            }

        }
    }
}
