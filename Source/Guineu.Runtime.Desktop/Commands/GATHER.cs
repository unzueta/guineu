using System;
using Guineu.Data;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu
{
	class GATHER : ICommand
	{
		ExpressionBase record;
		ExpressionBase memo;

		public void Compile(CodeBlock code)
		{
			var comp = new Compiler(null, code);

			Token nextToken = code.Reader.PeekToken();
			do
			{
				switch (nextToken)
				{
					case Token.NAME:
						code.Reader.ReadToken();
						record = comp.GetCompiledExpression();
						break;
					case Token.MEMO:
						code.Reader.ReadToken();
						memo = comp.GetCompiledExpression();
						break;
				}
				nextToken = code.Reader.PeekToken();
			} while (nextToken != Token.CmdEnd);
		}

		public void Do(CallingContext ctx, ref Int32 nextLine)
		{
			ICursor csr = ctx.DataSession.Cursor;
			if (csr == null)
				throw new ErrorException(ErrorCodes.NoTableOpen);

			Variant obj = record.GetVariant(ctx);
			if (obj.Type != VariantType.Object)
				throw new ErrorException(ErrorCodes.NotAnObject, record.GetName(ctx));

			ObjectBase o = obj;
			for (Int32 i = 0; i < o.Count; i++)
			{
				var name = o.GetMemberNameByPosition(i);
				var val = ((ValueMember)o.GetMember(name)).Get();
				csr.SetField(name, val);
			}
			csr.FlushRecord();
	
		}
	}
}