using Guineu.Expression;

namespace Guineu.Commands
{
    class SetStatusFacade : ICommand
    {
        ICommand cmd;

        public void Compile(CodeBlock code)
        {
            var tok = code.Reader.PeekToken();
            if (tok == Token.BAR)
            {
                code.Reader.ReadToken();
                cmd = new SETSTATUSBAR();
            }
            else
                cmd = new SETSTATUS();
            cmd.Compile(code);
        }

        public void Do(CallingContext context, ref int nextLine)
        {
            cmd.Do(context, ref nextLine);
        }
    }
}
