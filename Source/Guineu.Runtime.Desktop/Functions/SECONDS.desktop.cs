using System;

namespace Guineu.Functions
{
    partial class SECONDS
    {
        class TickProvider
        {
            static readonly bool UseWinMm;

            static TickProvider()
            {
                    try
                    {
                        timeGetTime();
                        UseWinMm = true;
                    }
                    catch (Exception)
                    {
                        UseWinMm = false;
                    }
            }

            [System.Runtime.InteropServices.DllImport("winmm", EntryPoint = "timeGetTime")]
            static extern uint timeGetTime();

            internal uint Ticks
            {
                get
                {
                    uint curTicks;
                    if(UseWinMm)
                        curTicks = timeGetTime();
                    else
                        curTicks = (uint)Environment.TickCount;
                    return curTicks;
                }
            }

        }
    }
}
