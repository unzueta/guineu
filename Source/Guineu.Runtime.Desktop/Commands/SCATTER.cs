using System;
using Guineu.Data;
using Guineu.Expression;
using Guineu.ObjectEngine;

namespace Guineu.Commands
{
	class SCATTER : ICommand
	{
		ExpressionBase nameClause;

		enum ScatterTo
		{
			MemVar,
			Object,
			Array
		}
		ScatterTo operation;
		
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
						nameClause = comp.GetCompiledExpression();
						operation = ScatterTo.Object;
						break;
					case Token.MEMO:
						code.Reader.ReadToken();
						break;
					case Token.MEMVAR:
						code.Reader.ReadToken();
						operation = ScatterTo.MemVar;
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

			IMemberList o;
			switch (operation)
			{
				case ScatterTo.MemVar:
					o = ctx.Locals;
					break;
				case ScatterTo.Object:
					o = new ObjectBase();
					break;
				case ScatterTo.Array:
					o = ctx.Locals;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var rec = csr.GetCurrentRecord();
			foreach (var column in csr.Columns)
			{
				var val = rec.GetMember(column.Name) as ValueMember;
				if (val == null)
					throw new ErrorException(ErrorCodes.InternalConsistency);
				var property = new ValueMember(val.Get());
				o.Set(column.Name, property);
			}

			if (operation == ScatterTo.Object)
			{
				var objName = nameClause.ToNti(ctx);
				var obj = new Variant(o as ObjectBase);
				ctx.Locals.Set(objName, obj);
			}
		}
	}
}