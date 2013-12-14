using System;
using System.Collections.Generic;
using System.IO;
using Guineu.Expression;

namespace Guineu
{

	class LineInfoSCAN : LineInfo
	{
		internal Int32 Area;
	}

	class SCAN : ICommand
	{
		int m_JumpEndOfLoop;
		int m_LineInfo;
		ExpressionBase forClause;

		public void Compile(CodeBlock code)
		{
			Compiler Comp = new Compiler(null, code);
			Token nextToken = code.Reader.PeekToken();
			do
			{
				switch (nextToken)
				{
					case Token.FOR:
						code.Reader.ReadToken();
						forClause = Comp.GetCompiledExpression();
						break;

					default:
						m_JumpEndOfLoop = code.GetLineAtPosition(Comp.ReadInt());
						m_LineInfo = code.CurrentLine;
						break;
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);

			code.ControlFlowStack.Push(code.CurrentLine, FlowControlEntry.Types.Scan, m_JumpEndOfLoop+1);
		}

		public void Do(CallingContext context, ref Int32 nextLine)
		{
			LineInfoSCAN info = context.GetLineInfo(m_LineInfo) as LineInfoSCAN;

			Boolean FirstCall = (info == null);
			if (FirstCall)
			{
				info = new LineInfoSCAN();
				info.Area = 1; // current work area
				context.DataSession.Cursor.GoToTop();
				while (!context.DataSession.Cursor.Eof())
				{
					if (forClause == null || forClause.GetBool())
						break;
					context.DataSession.Cursor.Skip(1);
				}
				context.SetLineInfo(m_LineInfo, info);
			}

			// TODO: Select work area

			// We are on EOF
			if (context.DataSession.Cursor.Eof())
			{
				nextLine = m_JumpEndOfLoop + 1;
				return;
			}
			if (!FirstCall)
			{
				if (forClause == null)
					context.DataSession.Cursor.Skip(1);
				else
					while (!context.DataSession.Cursor.Eof())
					{
						context.DataSession.Cursor.Skip(1);
						if (forClause == null || forClause.GetBool())
							break;
					}
				// We were on the last record
				if (context.DataSession.Cursor.Eof())
				{
					nextLine = m_JumpEndOfLoop + 1;
				}
			}

		}

	}

}
