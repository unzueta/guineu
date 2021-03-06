﻿using System;

namespace Guineu
{
    partial class ExecutionPath
    {
        static ErrorAction RespondToError(string name, string fileName, ErrorCodes err, string param, int line)
        {
            ErrorAction act = GuineuInstance.WinMgr.ShowErrorDialog(String.Format("{0}-Error: {1} in line {2} of {3} ({4})", err, param, line + 1, name, fileName));
            return act;
        }
        
    }
}