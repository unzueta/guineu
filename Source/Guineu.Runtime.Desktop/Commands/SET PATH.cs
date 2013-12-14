using System;
using System.Collections.Generic;
using Guineu.Expression;
using System.Collections;

namespace Guineu.Commands
{
    class SetPath : ICommand
    {
        ExpressionBase path;

        public void Compile(CodeBlock code)
        {
            var comp = new Compiler(null, code);
            Token nextToken;
            do
            {
                nextToken = code.Reader.ReadToken();
                switch (nextToken)
                {
                    case Token.TO:
                        path = comp.GetCompiledExpression();
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
            GuineuInstance.Set.Path.Value = path.GetString(context);
        }
    }

    public partial class SetPathValue : IEnumerable<String>
    {
        String path;
        readonly List<String> pathList = new List<string>();

        partial void ParsePath();

        public String Value
        {
            get { return path; }
            set { path = value; ParsePath(); }
        }

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return pathList.GetEnumerator();
        }

        #endregion
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return pathList.GetEnumerator();
        }

        #endregion
    }

}
