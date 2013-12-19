using System;
using System.Collections.Generic;
using Guineu.Commands;
using Guineu.Expression;

namespace Guineu
{

	enum CommandTokens
	{
		AMPERSAND = 0x01,
		PRINT = 0x02,
		PRINTPRINT = 0x03,
		APPEND = 0x06,
		CASE = 0x0C,
		CLEAR = 0x0E,
		CLOSE = 0x0F,
		CONTINUE = 0x10,
		COPY = 0x11,
		COUNT = 0x12,
		DELETE = 0x14,
		DIMENSION = 0x15,
		DO = 0x18,
		ENDCASE = 0x1C,
		ERASE = 0x20,
		EXIT = 0x21,
		GOTO = 0x23,
		LOCATE = 0x2D,
		LOOP = 0x2E,
		OTHERWISE = 0x32,
		PARAMETERS = 0x34,
		PRIVATE = 0x35,
		PUBLIC = 0x37,
		QUIT = 0x38,
		READ = 0x39,
		REPLACE = 0x3E,
		RESTORE = 0x40,
		RETURN = 0x42,
		RUN = 0x43,
		SAVE = 0x44,
		SEEK = 0x45,
		SELECT = 0x46,
		SET = 0x47,
		SKIP = 0x48,
		STORE = 0x4A,
		SUM = 0x4B,
		USE = 0x51,
		WAIT = 0x52,
		ZAP = 0x53,
		LET = 0x54,
		SCATTER = 0x5E,
		GATHER = 0x5F,
		CREATE = 0x68,
		INSERTSQL = 0x72,
		FunctionCall2 = 0x86,
		EXTERNAL = 0x90,
		FunctionCall = 0x99,
		ADD = 0x96, // ADD TABLE, ADD OBJECT, etc.
		AddMethod = 0xA2,
		AddProtectedMethod = 0xA3,
		ERROR = 0xA8,
		ASSERT = 0xA9,
		DEBUGOUT = 0xAA,
		NODEFAULT = 0xAC,
		LOCAL = 0xAE,
		LPARAMETERS = 0xAF,
		CD = 0xB0,
		MKDIR = 0xB1,
		DEBUG = 0xB3,

		// control statements
		IF = 0x25,
		ELSE = 0x1B,
		ENDDO = 0x1D,
		ENDIF = 0x1E,
		ENDPROC = 0x55,
		SCAN = 0x7E,
		ENDSCAN = 0x7F,
		FOR = 0x84,
		ENDFOR = 0x85,
	}

	class TokenList<T1, T2, T3> : Dictionary<KeyValuePair<T1, T2>, T3>
	{
		public T3 this[T1 index1, T2 index2]
		{
			get
			{
				return this[GetKey(index1, index2)];
			}
		}

		public void Add(T1 index1, T2 index2, T3 data)
		{
			Add(GetKey(index1, index2), data);
		}

		public Boolean ContainsKey(T1 index1, T2 index2)
		{
			return ContainsKey(GetKey(index1, index2));
		}

		static KeyValuePair<T1, T2> GetKey(T1 index1, T2 index2)
		{
			return new KeyValuePair<T1, T2>(index1, index2);
		}

	}

	// VS2010/.NET 4.0 Code
	//class TokenList<T1, T2, T3> : Dictionary<Tuple<T1, T2>, T3>
	//{
	//    public T3 this[T1 index1, T2 index2]
	//    {
	//        get
	//        {
	//            return this[Tuple.Create(index1, index2)];
	//        }
	//    }
	//    public void Add(T1 index1, T2 index2, T3 data)
	//    {
	//        Add(Tuple.Create(index1, index2), data);
	//    }
	//    public Boolean ContainsKey(T1 index1, T2 index2)
	//    {
	//        return ContainsKey(Tuple.Create(index1, index2));
	//    }
	//}

	class CommandFactory
	{
		readonly TokenList<CommandTokens, Int32, Func<ICommand>> list;
		const Int32 DefaultClause = 0;

		public CommandFactory()
		{
			list = new TokenList<CommandTokens, Int32, Func<ICommand>>();
		}

		public Int32 Default { get { return DefaultClause; } }

		public Boolean Contains(CommandTokens cmd, Int32 clause)
		{
			return list.ContainsKey(cmd, clause);
		}

		public ICommand Get(CommandTokens cmd, Token clause)
		{
			try
			{
				return list[cmd, (Int32)clause]();
			}
			catch (KeyNotFoundException)
			{
				if (clause == DefaultClause)
					throw new ErrorException(ErrorCodes.UnrecognizedCommand);
				throw new ErrorException(ErrorCodes.CommandIsMissingRequiredClause);
			}
		}
		public ICommand Get(CommandTokens cmd)
		{
			return Get(cmd, DefaultClause);
		}

		public void Register<TClass>(CommandTokens cmd) where TClass : ICommand, new()
		{
			Register(cmd, DefaultClause, () => new TClass());
		}
		public void Register<TClass>(CommandTokens cmd, Token clause) where TClass : ICommand, new()
		{
			Register(cmd, (Int32)clause, () => new TClass());
		}
		public void Register<TClass>(CommandTokens cmd, SetToken clause) where TClass : ICommand, new()
		{
			Register(cmd, (Int32)clause, () => new TClass());
		}
		void Register(CommandTokens cmd, Int32 clause, Func<ICommand> fnc)
		{
			list.Add(cmd, clause, fnc);
		}
	}

	public interface ICommand
	{
		void Compile(CodeBlock code);
		void Do(CallingContext context, ref int nextLine);
	}

	public partial class TokenCompiler
	{
		readonly CommandFactory cmd = new CommandFactory();

		public TokenCompiler()
		{
			cmd.Register<ADDOBJECT>(CommandTokens.ADD, Token.OBJECT);
			cmd.Register<AddMethod>(CommandTokens.AddMethod);
			cmd.Register<AMPERSAND>(CommandTokens.AMPERSAND);
			cmd.Register<ASSERT>(CommandTokens.ASSERT);
			cmd.Register<CASE>(CommandTokens.CASE);
			cmd.Register<CD>(CommandTokens.CD);
			cmd.Register<CLEAR>(CommandTokens.CLEAR);
			cmd.Register<CONTINUE>(CommandTokens.CONTINUE);
			cmd.Register<DEBUG>(CommandTokens.DEBUG);
			cmd.Register<DEBUGOUT>(CommandTokens.DEBUGOUT);
			cmd.Register<DELETE>(CommandTokens.DELETE);
			cmd.Register<DIMENSION>(CommandTokens.DIMENSION);
			cmd.Register<DO>(CommandTokens.DO);
			cmd.Register<DOCASE>(CommandTokens.DO, Token.CASE);
			cmd.Register<DOFORM>(CommandTokens.DO, Token.FORM);
			cmd.Register<DOWHILE>(CommandTokens.DO, Token.WHILE);
			cmd.Register<ELSE>(CommandTokens.ELSE);
			cmd.Register<ENDCASE>(CommandTokens.ENDCASE);
			cmd.Register<ENDDO>(CommandTokens.ENDDO);
			cmd.Register<ENDFOR>(CommandTokens.ENDFOR);
			cmd.Register<ENDIF>(CommandTokens.ENDIF);
			cmd.Register<ENDPROC>(CommandTokens.ENDPROC);
			cmd.Register<ENDSCAN>(CommandTokens.ENDSCAN);
			cmd.Register<ERROR>(CommandTokens.ERROR);
			cmd.Register<EXIT>(CommandTokens.EXIT);
			cmd.Register<EXTERNAL>(CommandTokens.EXTERNAL);
			cmd.Register<FOR>(CommandTokens.FOR);
			cmd.Register<FunctionCall>(CommandTokens.FunctionCall);
			cmd.Register<FunctionCall>(CommandTokens.FunctionCall2);
			cmd.Register<IF>(CommandTokens.IF);
			cmd.Register<LET>(CommandTokens.LET);
			cmd.Register<LPARAMETERS>(CommandTokens.LPARAMETERS);
			cmd.Register<LOCAL>(CommandTokens.LOCAL);
			cmd.Register<LOCATE>(CommandTokens.LOCATE);
			cmd.Register<LOOP>(CommandTokens.LOOP);
			cmd.Register<NODEFAULT>(CommandTokens.NODEFAULT);
			cmd.Register<OTHERWISE>(CommandTokens.OTHERWISE);
			cmd.Register<PARAMETERS>(CommandTokens.PARAMETERS);
			cmd.Register<PUBLIC>(CommandTokens.PUBLIC);
			cmd.Register<PRIVATE>(CommandTokens.PRIVATE);
			cmd.Register<RESTORE>(CommandTokens.RESTORE, Token.FROM);
			cmd.Register<RETURN>(CommandTokens.RETURN);
			cmd.Register<SAVE>(CommandTokens.SAVE, Token.TO);
			cmd.Register<STORE>(CommandTokens.STORE);
			cmd.Register<SetStatusFacade>(CommandTokens.SET, SetToken.STATUS);
			cmd.Register<SETSAFETY>(CommandTokens.SET, SetToken.SAFETY);
			cmd.Register<SETEXACT>(CommandTokens.SET, SetToken.EXACT);
			cmd.Register<SETTALK>(CommandTokens.SET, SetToken.TALK);
			cmd.Register<SETMEMOWIDTH>(CommandTokens.SET, SetToken.MEMOWIDTH);
			cmd.Register<SetPath>(CommandTokens.SET, SetToken.PATH);
			cmd.Register<SETEXCLUSIVE>(CommandTokens.SET, SetToken.EXCLUSIVE);
			cmd.Register<SETSYSFORMATS>(CommandTokens.SET, SetToken.SYSFORMATS);
			cmd.Register<SETDELETED>(CommandTokens.SET, SetToken.DELETED);
			cmd.Register<SETNULLDISPLAY>(CommandTokens.SET, SetToken.NULLDISPLAY);
			cmd.Register<SETSTEP>(CommandTokens.SET, SetToken.STEP);
			cmd.Register<SETCOVERAGE>(CommandTokens.SET, SetToken.COVERAGE);
			cmd.Register<SETBELL>(CommandTokens.SET, SetToken.BELL);
			cmd.Register<SETNOTIFY>(CommandTokens.SET, SetToken.NOTIFY);
			cmd.Register<SETDECIMALS>(CommandTokens.SET, SetToken.DECIMALS);
			cmd.Register<SETPROCEDURE>(CommandTokens.SET, SetToken.PROCEDURE);
			cmd.Register<SETCLASSLIB>(CommandTokens.SET, SetToken.CLASSLIB);
			cmd.Register<SETPOINT>(CommandTokens.SET, SetToken.POINT);
			FillCommands();
		}

		partial void FillCommands();

		internal ICommand Compile(CodeBlock code)
		{
			//---------------------------------------------------------------------------------
			// The first byte of each line is the command token
			//---------------------------------------------------------------------------------
			ICommand command;
			var token = (CommandTokens)code.Reader.ReadByte();
			var clause = code.Reader.PeekToken((Token)cmd.Default);
			if ((Int32)clause != cmd.Default && cmd.Contains(token, (Int32)clause))
			{
				code.Reader.ReadToken();
				command = cmd.Get(token, clause);
			}
			else
				command = cmd.Get(token);

			//---------------------------------------------------------------------------------
			// Depending on the token we create and compile a command.
			//---------------------------------------------------------------------------------
			if (command != null)
				command.Compile(code);
			return command;
		}
	}
}