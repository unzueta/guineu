using System;
using System.Collections.Generic;
using System.Text;
using Guineu.Expression;

namespace Guineu
{
    class SETEXACT : ICommand
    {
        Boolean Exact;

        public void Compile(CodeBlock code)
        {
            Token nextToken;
            do
            {
                nextToken = code.Reader.ReadToken();
                switch (nextToken)
                {
                    case Token.ON:
                        Exact = true;
                        break;
                    case Token.OFF:
                        Exact = false;
                        break;
                    case Token.CmdEnd:
                        break;
                    default:
                        // (...) Invalid token
                        break;
                }
            } while (nextToken != Token.CmdEnd);
        }

        public void Do(CallingContext context, ref int nextLine)
        {
            GuineuInstance.Set.Exact.Value = Exact;
        }
    }

    // TODO: SET DELETED is scoped to the current datasession
    public class SetExactValue
    {
        private Boolean _Exact;

        public Boolean Value
        {
            get { return _Exact; }

            set { _Exact = value; }
        }

    }

}
