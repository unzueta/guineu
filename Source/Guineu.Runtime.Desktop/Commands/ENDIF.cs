using System;

namespace Guineu.Commands
{
    class ENDIF : ICommand
    {
        int lineInfo;

        public void Compile(CodeBlock code)
        {
            lineInfo = code.ControlFlowStack.Pop(FlowControlEntry.Types.If);
        }

        public void Do(CallingContext context, ref Int32 nextLine)
        {
            context.SetLineInfo(lineInfo, null);
        }
    }
}
