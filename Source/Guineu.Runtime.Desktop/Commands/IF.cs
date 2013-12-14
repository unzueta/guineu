using System;
using Guineu.Expression;

namespace Guineu.Commands
{
    class LineInfoIf : LineInfo
    {
        internal bool ExecuteIf;
    }

    class IF : ICommand
    {
        ExpressionBase condition;
        int jumpElse;
        int lineInfo;

        public void Compile(CodeBlock code)
        {
            var comp = new Compiler(null, code);
            condition = comp.GetCompiledExpression();
            jumpElse = code.GetLineAtPosition(comp.ReadInt());
            lineInfo = code.CurrentLine;
            code.ControlFlowStack.Push(lineInfo, FlowControlEntry.Types.If);
        }

        public void Do(CallingContext context, ref Int32 nextLine)
        {
            var info = new LineInfoIf
                           {
                               ExecuteIf = condition.GetBool(context)
                           };
            context.SetLineInfo(lineInfo, info);

            if (!info.ExecuteIf)
                nextLine = jumpElse;
        }
    }
}
