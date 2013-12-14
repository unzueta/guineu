using System;
using Guineu.Expression;

namespace Guineu.Commands
{

    class ELSE : ICommand
    {
        int lineInfo;
        int jump;

        public void Compile(CodeBlock code)
        {
            var comp = new Compiler(null, code);
            jump = code.GetLineAtPosition(comp.ReadInt());
            lineInfo = code.ControlFlowStack.Pop(FlowControlEntry.Types.If);
            code.ControlFlowStack.Push(lineInfo, FlowControlEntry.Types.If);
        }

        public void Do(CallingContext context, ref Int32 nextLine)
        {
            var info = context.GetLineInfo(lineInfo) as LineInfoIf;
            if (info == null)
                throw new ErrorException(ErrorCodes.Syntax);
            if (info.ExecuteIf)
                nextLine = jump;
        }
    }
}
