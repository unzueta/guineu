using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Guineu.Data;
using Guineu.Functions;
using Guineu.ObjectEngine;
using System.Diagnostics;

namespace Guineu.Expression
{
	enum Token
	{
		// Data types
		ResultOfOperation = 0xCC,
		String = 0xD9,
		Int32 = 0xE9,
		Int8 = 0xF8,
		Int16 = 0xF9,
		Number = 0xFA,
		Date = 0xEE,
		DateTime = 0xE6,

		// expression elements
		ParamList = 0x43,
		SystemObject = 0xE1,
		ArrayAlias = 0xE5,
		SkipOnTrue = 0xF0,
		SkipOnFalse = 0xF1,
		SkipParamOnFalse = 0xF2,
		SkipParamOnTrue = 0xF3,
		Alias = 0xF4,
		WorkArea = 0xF5, // 01-0A: workareas A-J, OD: memory variable
		Array = 0xF6,
		Variable = 0xF7,
		Name = 0xFB,
		Expression = 0xFC,
		End = 0xFD,
		CmdEnd = 0xFE,
		ReturnDefault = 0xFF,
		ArrayMemberAccess = 0xE0,
		ReadArray = 0xEB,
		WriteArray = 0xEA19,

		// Constants
		True = 0x61,
		False = 0x2D,
		NULL = 0xE4,

		// operators
		ByVal = 0x00,
		Contains = 0x01,
		OpenParenthesis = 0x02,
		Parenthesis = 0x03,
		Multiply = 0x04,
		Power = 0x05,
		Add = 0x06,
		Comma = 0x07,
		Minus = 0x08,
		And = 0x09,
		Not = 0x0A,
		Or = 0x0B,
		Division = 0x0C,
		LessThan = 0x0D,
		LessOrEqualThan = 0x0E,
		NotEqual = 0x0F,
		Equals = 0x10,
		GreaterThan = 0x11,
		GreaterOrEqualThan = 0x12,
		EqualsBinary = 0x14,
		SquareBrackets = 0x16,
		ByRef = 0x18,
		Modulo = 0x47,

		// Command clauses
		// TODO: Move this into a separate enum
		ADDITIVE = 0x01,
		ALIAS_Clause = 0x02,
		ALL = 0x03,
		ARRAY = 0x04,
		BAR = 0x06,
		BLANK = 0x08,
		FIELDS = 0x11,
		FILE_Clause = 0x12,
		FOR = 0x13,
		FORM = 0x14,
		FROM = 0x15,
		IN = 0x16,
		MEMO = 0x1B,
		NEXT = 0x1E,
		OFF = 0x1F,
		ON = 0x20,
		RECORD = 0x23,
		REST = 0x24,
		TO = 0x28,
		TOP = 0x29,
		WHILE = 0x2B,
		WINDOW = 0x2C,
		OBJECT = 0x2E,
		NOOPTIMIZE = 0x30,
		TABLE = 0x31,
		BOTTOM = 0x36,
		CASE = 0x48,
		NAME = 0x4A,
		AS = 0x51,
		EXCLUSIVE = 0xBC,
		INTO = 0xBC,
		CURSOR = 0xBD,
		SHARED = 0xC2,
		MEMVAR = 0xC2,
		DATABASES = 0xC2,
		ORDER = 0xC3,
		RECYCLE = 0xC4,
		VALUES = 0xC5,
		STEP = 0xC7,
		WITH = 0xD1,
		EVENTS = 0xD5,

		// Functions
		ABS = 0x19,
		ALIAS = 0x1B,
		ASC = 0x1C,
		AT = 0x1D,
		BOF = 0x1E,
		CHR = 0x20,
		CMONTH = 0x21,
		CTOD = 0x23,
		DATE = 0x24,
		DAY = 0x25,
		DELETED = 0x27,
		DOW = 0x29,
		DTOC = 0x2A,
		EOF = 0x2B,
		// ERROR = 0x2C,
		FCOUNT = 0x2E,
		FIELD = 0x2F,
		FILE = 0x30,
		FOUND = 0x34,
		GETENV = 0x35,
		IIF = 0x36,
		INT = 0x38,
		ISALPHA = 0x39,
		ISLOWER = 0x3B,
		ISUPPER = 0x3C,
		LEFT = 0x3D,
		LEN = 0x3E,
		LOWER = 0x40,
		MAX = 0x44,
		MIN = 0x46,
		//		MOD = 0x47,
		MONTH = 0x48,
		OS = 0x4A,
		RECCOUNT = 0x4E,
		RECNO = 0x4F,
		REPLICATE = 0x51,
		RIGHT = 0x52,
		ROUND = 0x54,
		SELECT = 0x57,
		SPACE = 0x58,
		SQRT = 0x59,
		STR = 0x5A,
		STUFF = 0x5B,
		SUBSTR = 0x5C,
		SYS = 0x5D,
		TIME = 0x5E,
		TRANSFORM = 0x5F,
		UPPER = 0x66,
		VAL = 0x67,
		VERSION = 0x68,
		YEAR = 0x69,
		PROGRAM = 0x74,
		PV = 0x75,
		SET = 0x76,
		CEILING = 0x77,
		FLOOR = 0x7A,
		FV = 0x7B,
		PAYMENT = 0x83,
		RAND = 0x86,
		RTOD = 0x89,
		SEEK = 0x8A,
		SIGN = 0x8B,
		DTOR = 0x8D,
		DTOS = 0x8E,
		FOPEN = 0x90,
		FCLOSE = 0x91,
		FREAD = 0x92,
		FWRITE = 0x93,
		FERROR = 0x94,
		FCREATE = 0x95,
		FSEEK = 0x96,
		FGETS = 0x97,
		FFLUSH = 0x98,
		FPUTS = 0x99,
		ALLTRIM = 0x9B,
		CHRTRAN = 0x9D,
		FILTER = 0x9E,
		EMPTY = 0xA1,
		FEOF = 0xA2,
		RAT = 0xA5,
		SECONDS = 0xA7,
		STRTRAN = 0xA8,
		USED = 0xAA,
		BETWEEN = 0xAB,
		INLIST = 0xAD,
		ISDIGIT = 0xAE,
		OCCURS = 0xAF,
		PADC = 0xB0,
		PADL = 0xB1,
		PADR = 0xB2,
		ATC = 0xB8,
		CURDIR = 0xBA,
		FULLPATH = 0xBB,
		GOMONTH = 0xC3,
		PARAMETERS = 0xC4,
		FCHSIZE = 0xCB,
		ALEN = 0xCD,
		EVALUATE = 0xCE,
		ISNULL = 0xD1,
		NVL = 0xD2,

		// Escape codes
		ExtendedFunction = 0xEA,
		ExtendedFunction2 = 0x1A,
		ExtendedFunctionBase = 0xEA00,
		ExtendedFunctionBase2 = 0x1A00,

		// Extended functions
		ADEL = 0xEA0F,
		ADIR = 0xEA15,
		GETPEM = 0xEA21,
		FONTMETRIC = 0xEA24,
		SYSMETRIC = 0xEA25,
		SOUNDEX = 0xEA41,
		COS = 0xEA43,
		SIN = 0xEA44,
		TAN = 0xEA45,

		ACOS = 0xEA46,
		ASIN = 0xEA47,
		ATAN = 0xEA48,
		ATAN2 = 0xEA49,
		LOG = 0xEA4A,
		LOG10 = 0xEA4B,
		EXP = 0xEA4C,
		PI = 0xEA4D,
		CREATEOBJECT = 0xEA4E,
		FDATE = 0xEA58,
		FTIME = 0xEA59,
		RGB = 0xEA5E,
		SQLDISCONNECT = 0xEA67,
		SQLEXEC = 0xEA69,
		MESSAGEBOX = 0xEA78,
		AERROR = 0xEA7A,
		TTOD = 0xEA80,
		CTOT = 0xEA82,
		HOUR = 0xEA83,
		MINUTE = 0xEA84,
		SEC = 0xEA85,
		DATETIME = 0xEA86,
		APRINTERS = 0xEA99,
		SQLSTRINGCONNECT = 0xEA9F,

		BITLSHIFT = 0xEAA7,
		BITRSHIFT = 0xEAA8,
		BITAND = 0xEAA9,
		BITOR = 0xEAAA,
		BITNOT = 0xEAAB,
		BITXOR = 0xEAAC,
		BITSET = 0xEAAD,
		BITTEST = 0xEAAE,
		BITCLEAR = 0xEAAF,
		RATC = 0xEAB2,
		STRCONV = 0xEABE,
		DIRECTORY = 0xEAC6,
		STRTOFILE = 0xEACB,
		FILETOSTR = 0xEACC,
		ADDBS = 0xEACD,

		DEFAULTEXT = 0xEACE,
		DRIVETYPE = 0xEACF,
		FORCEEXT = 0xEAD0,
		FORCEPATH = 0xEAD1,

		JUSTDRIVE = 0xEAD2,
		JUSTEXT = 0xEAD3,
		JUSTFNAME = 0xEAD4,

		JUSTPATH = 0xEAD5,

		JUSTSTEM = 0xEAD6,
		VARTYPE = 0xEAD9,
		ALINES = 0xEADA,
		NEWOBJECT = 0xEADB,
		STREXTRACT = 0xEAF0,
		QUARTER = 0xEAEC,
		GETWORDCOUNT = 0xEAED,
		GETWORDNUM = 0xEAEE,

		// extended function 2
		DISPLAYPATH = 0x1A00,
		EVL = 0x1A0C,
		ICASE = 0x1A0E,
	}

	/// <summary>
	/// Represents a simple value in an expression tree
	/// </summary>
	/// <remarks>
	/// ExpressionBase 
	/// </remarks>
	public abstract class ExpressionBase
	{
		internal bool FixedInt;

		/// <summary>
		/// Read the expression from a stream
		/// </summary>
		/// <param name="c">Reference to a Compiler that points to the stream</param>
		internal abstract void Compile(Compiler c);
		internal abstract Variant GetVariant(CallingContext context);

		internal virtual int GetInt(CallingContext context) { return GetVariant(context); }
		internal virtual double GetDouble(CallingContext context) { return GetVariant(context); }

		internal virtual string GetString(CallingContext context)
		{
			Variant val = GetVariant(context);
			String ret = val.ToString(context.Context);
			return ret;
		}

		internal virtual bool GetBool(CallingContext context) { return GetVariant(context); }
		internal bool GetBool()
		{
			return GetBool(GuineuInstance.Context.CurrentContext);
		}

		public virtual ValueMember GetParameterValue(CallingContext ctx)
		{
			return new ValueMember(GetVariant(ctx));
		}

		// Returns null when the expression is not a name
		// Expressions that define a name (such as variables) need to override this method
		internal virtual String GetName(CallingContext context)
		{
			Variant val = GetVariant(context);
			if (val.Type == VariantType.Character)
				return val;
			return null;
		}

		internal virtual int GetStringLength(CallingContext context)
		{
			String str = GetString(context);
			if (str == null)
				return 0;
			return str.Length;
		}

		internal virtual VariantType GetVarType(CallingContext context)
		{
			return GetVariant(context).Type;
		}

		internal virtual Boolean IsNull(CallingContext context)
		{
			return GetVariant(context).IsNull;
		}

		/// <summary>
		/// Returns the member this expression currently represents when it is used in an assignment operation.
		/// </summary>
		/// <param name="context">calling context</param>
		/// <returns>A ValueMember if this expression can be used to store values, otherwise null.</returns>
		internal virtual ValueMember GetValueMember(CallingContext context)
		{
			return null;
		}

		/// <summary>
		/// Ensures that the passed value is a string
		/// </summary>
		/// <param name="context"></param>
		/// <param name="allowNull"></param>
		internal Boolean CheckString(CallingContext context, Boolean allowNull)
		{
			Variant var = GetVariant(context);
			if (allowNull && var.IsNull)
			{
				return true;
			}
			if (var.Type != VariantType.Character)
			{
				throw new ErrorException(ErrorCodes.InvalidArgument);
			}
			return var.IsNull;
		}

		/// <summary>
		/// Returns the cursor when this expression is a field reference.
		/// </summary>
		/// <param name="exec"></param>
		/// <returns></returns>
		internal virtual ICursor GetCursor(CallingContext exec)
		{
			return null;
		}
		/// <summary>
		/// Returns true when the expression has an additional qualifier.
		/// </summary>
		/// <returns></returns>
		internal virtual Boolean HasQualifier()
		{
			return false;
		}

		internal virtual Nti ToNti(CallingContext c)
		{
			String s = GetName(c);
			return new Nti(s);
		}
	}

	/// <summary>
	/// ExpressionQualifiers alter the meaning of the following token. Normally VFP uses a reverse
	/// polish notation for all expressions. 1+2 is stored as 1 2 +. However, some qualifiers such
	/// as "m.", alias names, etc. are stored in regular order. When parsing these tokens, we need
	/// to deal with qualifiers at compile time. Otherwise these values are removed from the stack
	/// when an expression list is requested.
	/// </summary>
	interface IExpressionQualifier
	{
		MemberResolver GetResolver(CallingContext context);
	}

	partial class Compiler
	{
		internal BinaryReader Reader;
		internal CallingContext Executor;
		internal Stack<ExpressionBase> Stack;
		internal Stack<int> Param;
		internal CodeBlock Code;
		static readonly Func<ExpressionBase>[] Functions = new Func<ExpressionBase>[255];
		static readonly Func<ExpressionBase>[] FunctionsEa = new Func<ExpressionBase>[255];

		internal Compiler(CallingContext ex, CodeBlock cb)
		{
			Executor = ex;
			Reader = cb.Reader;
			Code = cb;
			Stack = new Stack<ExpressionBase>();
			Param = new Stack<int>();
			FillFunctions();
		}
		partial void FillFunctions();

		static void Add<T>(Token token) where T : ExpressionBase, new()
		{
			var index = (Int32)token;
			if (index <= 255)
				Functions[index] = () => new T();
			else
				FunctionsEa[index - 0xEA00] = () => new T();
		}


		public CallingContext Context
		{
			get { return Executor; }
		}


		public ExpressionBase GetCompiledExpression()
		{
			if (Reader.BaseStream.Length == 0)
				return null;
			int level = 0;
			bool continueLoop;
			ExpressionBase exp;
			Token tok;
			do
			{
				continueLoop = false;
				exp = null;
				byte byteToken = Reader.ReadByte();
				tok = (Token)byteToken;
				#region switch token
				switch (tok)
				{
					case Token.ByVal:
						exp = new ByVal();
						break;
					case Token.ByRef:
						exp = new ByRef();
						break;
					case Token.Int8:
						exp = new IntConstant(8);
						break;
					case Token.Int16:
						exp = new IntConstant(16);
						break;
					case Token.Int32:
						exp = new IntConstant(32);
						break;
					case Token.Date:
						exp = new DateConstant();
						break;
					case Token.DateTime:
						exp = new DateTimeConstant();
						break;
					case Token.Expression:
						level++;
						break;
					case Token.End:
						level--;
						break;
					case Token.ParamList:
						Param.Push(Stack.Count);
						break;
					case Token.SECONDS:
						exp = new SECONDS();
						break;
					case Token.Variable:
						exp = new Variable();
						break;
					case Token.Add:
						exp = new OperatorAdd();
						break;
					case Token.Minus:
						exp = new OperatorMinus();
						break;
					case Token.Contains:
						exp = new OperatorContains();
						break;
					case Token.GreaterThan:
						exp = new OperatorGreaterThan();
						break;
					case Token.LessThan:
						exp = new OperatorLessThan();
						break;
					case Token.LessOrEqualThan:
						exp = new OperatorLessOrEqualThan();
						break;
					case Token.String:
						exp = new StringLiteral();
						break;
					case Token.Array:
						if (Param.Count == 0)
							exp = new ArrayDefinition();
						else
							exp = new ArrayAccess();
						break;
					case Token.CmdEnd:
						break;
					case Token.ALLTRIM:
						exp = new ALLTRIM();
						break;
					case Token.CHRTRAN:
						exp = new CHRTRAN();
						break;
					case Token.FULLPATH:
						exp = new FULLPATH();
						break;
					case Token.STR:
						exp = new STR();
						break;
					case Token.LEN:
						exp = new LEN();
						break;
					case Token.SPACE:
						exp = new SPACE();
						break;
					case Token.SQRT:
						exp = new SQRT();
						break;
					case Token.Parenthesis:
						exp = new Parenthesis();
						break;
					case Token.Multiply:
						exp = new OperatorMultiply();
						break;
					case Token.Division:
						exp = new OperatorDivision();
						break;
					case Token.Power:
						exp = new OperatorPower();
						break;
					case Token.Modulo:
						exp = new OperatorModulo();
						break;
					case Token.Number:
						exp = new NumberConstant();
						break;
					case Token.Equals:
						exp = new OperatorEquals();
						break;
					case Token.EqualsBinary:
						exp = new OperatorEqualsBinary();
						break;
					case Token.TRANSFORM:
						exp = new TRANSFORM();
						break;
					case Token.STUFF:
						exp = new STUFF();
						break;
					case Token.SUBSTR:
						exp = new SUBSTR();
						break;
					case Token.REPLICATE:
						exp = new REPLICATE();
						break;
					case Token.RIGHT:
						exp = new RIGHT();
						break;
					case Token.ROUND:
						exp = new ROUND();
						break;
					case Token.ISALPHA:
						exp = new ISALPHA();
						break;
					case Token.ISDIGIT:
						exp = new ISDIGIT();
						break;
					case Token.OCCURS:
						exp = new OCCURS();
						break;
					case Token.PADC:
						exp = new PADC();
						break;
					case Token.PADL:
						exp = new PADL();
						break;
					case Token.PADR:
						exp = new PADR();
						break;
					case Token.ISLOWER:
						exp = new ISLOWER();
						break;
					case Token.ISUPPER:
						exp = new ISUPPER();
						break;
					case Token.LEFT:
						exp = new LEFT();
						break;
					case Token.GreaterOrEqualThan:
						exp = new OperatorGreaterOrEqualThan();
						break;
					case Token.CHR:
						exp = new CHR();
						break;
					case Token.WorkArea:
						continueLoop = true;
						exp = new WorkArea();
						break;
					case Token.ResultOfOperation:
						break;
					case Token.INT:
						exp = new INT();
						break;
					case Token.DATE:
						exp = new DATE();
						break;
					case Token.DAY:
						exp = new DAY();
						break;
					case Token.MAX:
						exp = new MAX();
						break;
					case Token.MIN:
						exp = new MIN();
						break;
					//					case Token.MOD:
					//						exp = new MOD();
					//						break;
					case Token.MONTH:
						exp = new MONTH();
						break;
					case Token.YEAR:
						exp = new YEAR();
						break;
					case Token.VERSION:
						exp = new VERSION();
						break;
					case Token.DTOS:
						exp = new DTOS();
						break;
					case Token.EVALUATE:
						exp = new EVALUATE();
						break;
					case Token.NVL:
						exp = new NVL();
						break;
					case Token.GOMONTH:
						exp = new GOMONTH();
						break;
					case Token.ISNULL:
						exp = new ISNULL();
						break;
					case Token.OS:
						exp = new OS();
						break;
					case Token.SkipOnFalse:
						exp = new SkipOnFalse();
						break;
					case Token.SkipOnTrue:
						exp = new SkipOnTrue();
						break;
					case Token.SkipParamOnFalse:
						// These expressions are used in IIF() and ICASE() to handle navigation in the parameter list. 
						// We can safely ignore them, as we read the parameter list in an array anyway.
						Reader.ReadInt16();
						break;
					case Token.SkipParamOnTrue:
						Reader.ReadInt16();
						break;
					case Token.ReturnDefault:
						exp = new ReturnDefault();
						break;
					#region extended token 2
					case Token.ExtendedFunction2:
						var tokEx2 = Token.ExtendedFunctionBase2 + Reader.ReadByte();
						switch (tokEx2)
						{
							case Token.DISPLAYPATH:
								exp = new DISPLAYPATH();
								break;
							case Token.EVL:
								exp = new EVL();
								break;
							case Token.ICASE:
								exp = new ICASE();
								break;

							default:
								if (!GuineuInstance.IgnoreUnknownTokens)
								{
									throw new ErrorException(ErrorCodes.Syntax, "unknown extended token 2" + tokEx2 + " (" + tokEx2.ToString("x") + ")");
								}
								break;
						}
						break;
					#endregion extended token 2
					#region extended function EA
					case Token.ExtendedFunction:
						var tokEx = Token.ExtendedFunctionBase + Reader.ReadByte();
						switch (tokEx)
						{
							case Token.HOUR:
								exp = new HOUR();
								break;
							case Token.MINUTE:
								exp = new MINUTE();
								break;
							case Token.SEC:
								exp = new SEC();
								break;
							case Token.DATETIME:
								exp = new DATETIME();
								break;
							case Token.CREATEOBJECT:
								exp = new CREATEOBJECT();
								break;
							case Token.DIRECTORY:
								exp = new DIRECTORY();
								break;
							case Token.FILETOSTR:
								exp = new FILETOSTR();
								break;
							case Token.ACOS:
								exp = new ACOS();
								break;
							case Token.ASIN:
								exp = new ASIN();
								break;
							case Token.ATAN:
								exp = new ATAN();
								break;
							case Token.ATAN2:
								exp = new ATN2();
								break;
							case Token.SOUNDEX:
								exp = new SOUNDEX();
								break;
							case Token.COS:
								exp = new COS();
								break;
							case Token.SIN:
								exp = new SIN();
								break;
							case Token.TAN:
								exp = new TAN();
								break;
							case Token.LOG:
								exp = new LOG();
								break;
							case Token.LOG10:
								exp = new LOG10();
								break;
							case Token.EXP:
								exp = new EXP();
								break;
							case Token.PI:
								exp = new PI();
								break;
							case Token.BITAND:
								exp = new BITAND();
								break;
							case Token.BITCLEAR:
								exp = new BITCLEAR();
								break;
							case Token.BITLSHIFT:
								exp = new BITLSHIFT();
								break;
							case Token.BITNOT:
								exp = new BITNOT();
								break;
							case Token.BITOR:
								exp = new BITOR();
								break;
							case Token.BITRSHIFT:
								exp = new BITRSHIFT();
								break;
							case Token.BITSET:
								exp = new BITSET();
								break;
							case Token.BITTEST:
								exp = new BITTEST();
								break;
							case Token.BITXOR:
								exp = new BITXOR();
								break;
							case Token.RATC:
								exp = new RATC();
								break;
							case Token.ALINES:
								exp = new ALINES();
								break;
							case Token.NEWOBJECT:
								exp = new NEWOBJECT();
								break;
							case Token.STREXTRACT:
								exp = new STREXTRACT();
								break;
							case Token.QUARTER:
								exp = new QUARTER();
								break;
							case Token.GETWORDCOUNT:
								exp = new GETWORDCOUNT();
								break;
							case Token.GETWORDNUM:
								exp = new GETWORDNUM();
								break;
							case Token.JUSTPATH:
								exp = new JUSTPATH();
								break;
							case Token.ADDBS:
								exp = new ADDBS();
								break;
							case Token.DEFAULTEXT:
								exp = new DEFAULTEXT();
								break;
							case Token.DRIVETYPE:
								exp = new DRIVETYPE();
								break;
							case Token.FORCEEXT:
								exp = new FORCEEXT();
								break;
							case Token.FORCEPATH:
								exp = new FORCEPATH();
								break;
							case Token.JUSTDRIVE:
								exp = new JUSTDRIVE();
								break;
							case Token.JUSTEXT:
								exp = new JUSTEXT();
								break;
							case Token.JUSTFNAME:
								exp = new JUSTFNAME();
								break;
							case Token.JUSTSTEM:
								exp = new JUSTSTEM();
								break;
							case Token.RGB:
								exp = new RGB();
								break;
							case Token.WriteArray:
								exp = new WriteArray();
								break;
							case Token.VARTYPE:
								exp = new VARTYPE();
								break;
							case Token.AERROR:
								exp = new AERROR();
								break;
							case Token.GETPEM:
								exp = new GETPEM();
								break;
							case Token.TTOD:
								exp = new TTOD();
								break;
							case Token.CTOT:
								exp = new CTOT();
								break;
							case Token.ADEL:
								exp = new ADEL();
								break;
							case Token.STRCONV:
								exp = new STRCONV();
								break;
							default:
								if (FunctionsEa[(tokEx - Token.ExtendedFunctionBase)] == null)
								{
									if (!GuineuInstance.IgnoreUnknownTokens)
										throw new ErrorException(ErrorCodes.Syntax,
																						 "unknown extended token " + tokEx + " (" + tokEx.ToString("x") + ")");
									break;
								}
								exp = FunctionsEa[(tokEx - Token.ExtendedFunctionBase)]();
								break;
						}
						break;
					#endregion extended token
					case Token.ReadArray:
						exp = new ReadArray();
						break;
					case Token.RECCOUNT:
						exp = new RECCOUNT();
						break;
					case Token.RECNO:
						exp = new RECNO();
						break;
					case Token.ALIAS:
						exp = new ALIAS();
						break;
					case Token.PROGRAM:
						exp = new PROGRAM();
						break;
					case Token.PV:
						exp = new PV();
						break;
					case Token.CEILING:
						exp = new CEILING();
						break;
					case Token.FLOOR:
						exp = new FLOOR();
						break;
					case Token.FV:
						exp = new FV();
						break;
					case Token.PAYMENT:
						exp = new PAYMENT();
						break;
					case Token.RAND:
						exp = new RAND();
						break;
					case Token.RTOD:
						exp = new RTOD();
						break;
					case Token.SIGN:
						exp = new SIGN();
						break;
					case Token.DTOR:
						exp = new DTOR();
						break;
					case Token.UPPER:
						exp = new UPPER();
						break;
					case Token.VAL:
						exp = new VAL();
						break;
					case Token.CURDIR:
						exp = new CURDIR();
						break;
					case Token.Name:
						exp = new Name();
						break;
					case Token.BETWEEN:
						exp = new BETWEEN();
						break;
					case Token.STRTRAN:
						exp = new STRTRAN();
						break;
					case Token.INLIST:
						exp = new INLIST();
						break;
					case Token.ABS:
						exp = new ABS();
						break;
					case Token.AT:
						exp = new AT();
						break;
					case Token.ATC:
						exp = new ATC();
						break;
					case Token.Alias:
						continueLoop = true;
						exp = new Alias();
						break;
					case Token.True:
						exp = new ConstantTrue();
						break;
					case Token.False:
						exp = new ConstantFalse();
						break;
					case Token.NULL:
						exp = new ConstantNull();
						break;
					case Token.Not:
						exp = new OperatorNot();
						break;
					case Token.And:
						exp = new OperatorAnd();
						break;
					case Token.Or:
						exp = new OperatorOr();
						break;
					case Token.FILE:
						exp = new FILE();
						break;
					case Token.IIF:
						exp = new IIF();
						break;
					case Token.SYS:
						exp = new SYS();
						break;
					case Token.LOWER:
						exp = new LOWER();
						break;
					case Token.EMPTY:
						exp = new EMPTY();
						break;
					case Token.RAT:
						exp = new RAT();
						break;
					case Token.ALEN:
						exp = new ALEN();
						break;
					case Token.FCOUNT:
						exp = new FCOUNT();
						break;
					case Token.FIELD:
						exp = new FIELD();
						break;
					case Token.GETENV:
						exp = new GETENV();
						break;
					case Token.SELECT:
						exp = new SelectFunction();
						break;
					case Token.USED:
						exp = new USED();
						break;
					case Token.FOUND:
						exp = new FOUND();
						break;
					case Token.DTOC:
						exp = new DTOC();
						break;
					case Token.CTOD:
						exp = new CTOD();
						break;
					case Token.TIME:
						exp = new TIME();
						break;
					case Token.EOF:
						exp = new EOF();
						break;
					case Token.NotEqual:
						exp = new OperatorNotEquals();
						break;
					case Token.DELETED:
						exp = new DELETED();
						break;
					case Token.CMONTH:
						exp = new CMONTH();
						break;
					case Token.BOF:
						exp = new BOF();
						break;
					case Token.DOW:
						exp = new DOW();
						break;
					case Token.PARAMETERS:
						exp = new PARAMETERS();
						break;
					case Token.FILTER:
						exp = new FILTER();
						break;
					case Token.SET:
						exp = new SetFunction();
						break;
					case Token.ArrayMemberAccess:
						exp = new ArrayMemberAccess();
						break;
					default:
						if (Functions[(Int32)tok] == null)
						{
							if (!GuineuInstance.IgnoreUnknownTokens)
								throw new ErrorException(ErrorCodes.Syntax, "unknown token " + tok + " (" + tok.ToString("x") + ")");
							break;
						}
						exp = Functions[(Int32)tok]();
						break;
				}
				#endregion switch token
				if (exp != null)
				{
					exp.Compile(this);
					Stack.Push(exp);
				}
			} while (level > 0 || continueLoop);

			// If there wasn't an expresssion available, we return null
			ExpressionBase result = Stack.Count == 0 ? null : Stack.Pop();

			//// RETURN TO causes every exception to become .T. 
			//catch (ReturnToExeception)
			//{
			//  Stack.Clear();
			//  retVal = new Variant(true);
			//}

			// If there's anything left on the stack, this would be an error condition.

			return result;
		}

		internal Variant Read()
		{
			ExpressionBase value = GetCompiledExpression();
			return value.GetVariant(null);
		}

		internal int ReadInt()
		{
			ExpressionBase value = GetCompiledExpression();
			return value.GetInt(null);
		}

		public List<ExpressionBase> GetParameterList()
		{
			int limit = Param.Pop();
			var retList = new List<ExpressionBase>();
			while (Stack.Count > limit)
			{
				retList.Insert(0, Stack.Pop());
			}
			return retList;
		}
	}

	class Variable : ExpressionBase
	{
		Nti nti;
		string name;
		int indexValue;
		ExpressionBase qualifier;
		MemberResolver resolver;

		internal int Index
		{
			get { return indexValue; }
		}

		override internal void Compile(Compiler ec)
		{
			// A variable token can be preceded by a scope indicater such as a 
			// workarea or an object reference
			if (ec.Stack.Count >= 1)
			{
				ExpressionBase testExp = ec.Stack.Peek();
				if (testExp is IExpressionQualifier)
				{
					qualifier = ec.Stack.Pop();
				}
			}
			indexValue = ec.Reader.ReadInt16();
			name = ec.Code.GetName(indexValue);
			nti = new Nti(name);
		}

		internal override string GetName(CallingContext exec)
		{
			return name;
		}

		/// <summary>
		/// Returns the cursor if the qualifier is an alias Name.
		/// </summary>
		/// <param name="exec"></param>
		/// <returns>ICursor for the alias, or null if no alias is specified</returns>
		internal override ICursor GetCursor(CallingContext exec)
		{
			if (HasQualifier())
				return qualifier.GetCursor(exec);
			return null;
		}

		internal override bool HasQualifier()
		{
			return qualifier != null;
		}

		private MemberResolver GetResolver(CallingContext context)
		{
			var expressionQualifier = qualifier as IExpressionQualifier;
			if (expressionQualifier == null)
				return context.Resolver;

			MemberResolver memberResolver = expressionQualifier.GetResolver(context);
			if (memberResolver == null)
				throw new ErrorException(ErrorCodes.UnknownMember, qualifier.GetName(context));
			return memberResolver;
		}

		internal IMemberList GetResolverObject(CallingContext context)
		{
			var aliasQualifier = qualifier as Alias;
			if (aliasQualifier == null)
				return null;

			return aliasQualifier.GetResolverObject(context);
		}

		internal override Variant GetVariant(CallingContext exec)
		{
			switch (nti.ToKnownNti())
			{
				case KnownNti.This:
					return new Variant(exec.This);
				case KnownNti.ThisForm:
					return new Variant(exec.This.GetThisForm());
				case KnownNti.ThisFormSet:
					return new Variant(exec.This.GetThisFormSet());
				default:
					MemberResolver memberResolver = GetResolver(exec);
					Member mbr = memberResolver.Resolve(exec, nti);

					var val = mbr as ValueMember;
					if (val != null)
						return val.Get();

					var mth = mbr as MethodMember;
					if (mth != null)
						return mth.Execute(exec, new ParameterCollection());

					return new Variant((false));
			}
		}

		internal override ValueMember GetValueMember(CallingContext context)
		{
			MemberResolver memberResolver;
			if (qualifier == null)
			{
				if (resolver == null)
				{
					resolver = new VariableResolver();
				}
				memberResolver = resolver;
			}
			else
			{
				memberResolver = GetResolver(context);
			}
			Member mbr = memberResolver.Resolve(context, nti);
			return mbr as ValueMember;
		}

		internal override VariantType GetVarType(CallingContext context)
		{
			Variant val = GetVariant(context);
			switch (val.Type)
			{
				case VariantType.Integer:
					return VariantType.Number;
				default:
					return val.Type;
			}
		}

		public override string ToString()
		{
			String s = "";
			if (qualifier != null)
				s = qualifier + ".";
			return s + name;
		}
	}

	class Name : ExpressionBase
	{
		string strValue;

		override internal void Compile(Compiler comp)
		{
			int length = comp.Reader.ReadUInt16();
			byte[] value = comp.Reader.ReadBytes(length);
			// (...) Determine code page from FXP file
			char[] charValue = GuineuInstance.CurrentCp.GetChars(value);
			strValue = new String(charValue);
			if (strValue.Length >= 2)
				if (strValue[0] == '"' && strValue[strValue.Length - 1] == '"')
					strValue = strValue.Substring(1, strValue.Length - 2);

		}

		internal override string GetString(CallingContext exec)
		{
			return strValue;
		}

		internal override Variant GetVariant(CallingContext exec)
		{
			return new Variant(strValue);
		}

		internal override string GetName(CallingContext exec)
		{
			return strValue;
		}

		public override string ToString()
		{
			return strValue;
		}
	}

	class IntConstant : ExpressionBase
	{
		Variant value;
		readonly int bitCount;

		internal IntConstant(int bitCount)
		{
			this.bitCount = bitCount;
			FixedInt = true;
		}

		override internal void Compile(Compiler comp)
		{
			int intValue;
			int length = comp.Reader.ReadByte();
			switch (bitCount)
			{
				case 8:
					intValue = comp.Reader.ReadByte();
					break;
				case 16:
					intValue = comp.Reader.ReadInt16();
					break;
				case 32:
					intValue = comp.Reader.ReadInt32();
					break;
				default:
					throw new ErrorException(ErrorCodes.DataTypeInvalid);
			}
			// VFP has a minimum length of 10 digits for every number
			if (length < 10)
				length = 10;
			value = new Variant(intValue, length);
		}

		internal override int GetInt(CallingContext exec)
		{
			return value;
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			return value;
		}

		internal override double GetDouble(CallingContext exec)
		{
			return value;
		}

		internal override bool GetBool(CallingContext exec)
		{
			return value;
		}

		public override string ToString()
		{
			return value;
		}
	}


	class StringLiteral : ExpressionBase
	{
		string strValue;

		override internal void Compile(Compiler comp)
		{
			int length = comp.Reader.ReadUInt16();
			byte[] value = comp.Reader.ReadBytes(length);
			// (...) Determine code page from FXP file
			char[] charValue = GuineuInstance.CurrentCp.GetChars(value);
			strValue = new String(charValue);
		}

		internal override string GetString(CallingContext exec)
		{
			return strValue;
		}

		internal override Variant GetVariant(CallingContext exec)
		{
			return new Variant(strValue);
		}

		internal override string GetName(CallingContext exec)
		{
			return strValue;
		}

		public override string ToString()
		{
			return "\"" + strValue + "\"";
		}
	}

	/// <summary>
	/// ArrayAccess is used to access array elements and to call functions.
	/// </summary>
	class ArrayAccess : ExpressionBase
	{
		List<ExpressionBase> parms;
		Nti name;
		int methodIndex;
		ExpressionBase qualifier;

		CompiledProgram fxp;
		CodeBlock code;
		ObjectBase thisObject;

		internal override void Compile(Compiler er)
		{
			if (er.Stack.Count >= 1)
			{
				ExpressionBase testExp = er.Stack.Peek();
				if (testExp is IExpressionQualifier)
				{
					qualifier = er.Stack.Pop();
				}
			}
			parms = er.GetParameterList();
			methodIndex = er.Reader.ReadInt16();
			name = er.Code.GetNti(methodIndex);
		}

		internal override string GetName(CallingContext context)
		{
			return name.ToString();
		}

		internal override Nti ToNti(CallingContext c)
		{
			return name;
		}

		internal override string GetString(CallingContext context)
		{
			Variant val = GetVariant(context);
			String ret = val.ToString(context.Context);
			return ret;
		}

		internal override Variant GetVariant(CallingContext context)
		{
			var resolver = GetResolver(context);
			var v1 = resolver.Resolve(context, name) as ArrayMember;
			if (v1 == null)
				return CallUdf(context);
			Variant retVal = GetArrayValue(context, v1);
			return retVal;
		}

		internal override ValueMember GetValueMember(CallingContext context)
		{
			MemberResolver resolver = GetResolver(context);
			var v1 = resolver.Resolve(context, name) as ArrayMember;
			if (v1 == null)
				throw new ErrorException(ErrorCodes.VariableNotFound);

			ValueMember retVal = Locate(context, v1);
			return retVal;
		}

		Variant GetArrayValue(CallingContext context, ArrayMember v1)
		{
			Variant retVal = Locate(context, v1).Get();
			return retVal;
		}

		ValueMember Locate(CallingContext context, ArrayMember v1)
		{
			ValueMember val;
			switch (parms.Count)
			{
				case 0:
					throw new ErrorException(ErrorCodes.Syntax);
				case 1:
					val = v1.Locate(1, parms[0].GetInt(context));
					break;
				case 2:
					val = v1.Locate(parms[0].GetInt(context), parms[1].GetInt(context));
					break;
				default:
					// Additional parameters are ignored.
					val = v1.Locate(parms[0].GetInt(context), parms[1].GetInt(context));
					break;
			}
			return val;
		}

		private Variant CallUdf(CallingContext context)
		{
			// Parameters
			var param = new ParameterCollection();
			foreach (ExpressionBase p in parms)
				param.Add(p.GetParameterValue(context));

			Variant retVal;
			var expressionQualifier = qualifier as IExpressionQualifier;
			if (expressionQualifier == null)
				retVal = ResolveProcedure(context, param);
			else
				retVal = ResolveMethod(context, expressionQualifier, param);
			// (...) ObjectContext
			return retVal;
		}

		private Variant ResolveMethod(CallingContext context, IExpressionQualifier expressionQualifier, ParameterCollection param)
		{
			var resolver = expressionQualifier.GetResolver(context) as MemberListResolver;

			if (resolver == null)
			{
				string objectName = qualifier.GetName(context);
				throw new ErrorException(ErrorCodes.ObjectNotFound, objectName);
			}

			var method = resolver.Resolve(context, name) as MethodMember;
			if (method == null)
				throw new ErrorException(ErrorCodes.NotAnArray, name.ToString());

			Variant retVal = method.Execute(context, param);
			thisObject = resolver.GetThis();
			return retVal;
		}

		private Variant ResolveProcedure(CallingContext context, ParameterCollection param)
		{
			Variant retVal;
			thisObject = null;
			if (fxp == null)
			{
				var resolver = new ProcedureResolver();
				fxp = resolver.FindProcedure(context, name.ToString());
			}
			if (fxp == null)
				throw new ErrorException(ErrorCodes.FileNotFound, name + ".prg");

			code = fxp.Locate(name);
			if (code == null)
				retVal = new Variant(true);
			else
				retVal = context.Context.ExecuteInNewContext(code, param, thisObject);
			return retVal;
		}

		private MemberResolver GetResolver(CallingContext context)
		{
			var expressionQualifier = qualifier as IExpressionQualifier;
			if (expressionQualifier == null)
				return context.Resolver;

			return expressionQualifier.GetResolver(context);
		}

		public override string ToString()
		{
			String s = "";
			if (qualifier != null)
				s = qualifier + ".";
			s = s + name + "(";

			for (var i = 0; i < parms.Count; i++)
			{
				if (i > 0)
					s = s + ",";
				s = s + parms[i];
			}
			return s + ")";
		}
	}

	/// <summary>
	/// Defines an array, for instance in a LOCAL statement.
	/// </summary>
	class ArrayDefinition : ExpressionBase
	{
		string name;
		int indexValue;
		ExpressionBase dimension1;
		ExpressionBase dimension2;
		int dimensions;
		ExpressionBase qualifier;
		MemberResolver resolver;

		internal override void Compile(Compiler er)
		{
			// An array definition can be preceded by an object reference
			if (er.Stack.Count >= 1)
			{
				ExpressionBase testExp = er.Stack.Peek();
				if (testExp is IExpressionQualifier)
				{
					qualifier = er.Stack.Pop();
				}
			}

			// Read the array Name
			indexValue = er.Reader.ReadInt16();
			name = er.Code.GetName(indexValue);

			// The array Name is followed by one or two dimensions, followed by a closing
			// parenthesis.
			dimension1 = er.GetCompiledExpression();
			var tok = (Token)er.Reader.ReadByte();
			if (tok == Token.Comma)
			{
				dimension2 = er.GetCompiledExpression();
				er.Reader.ReadByte();
				dimensions = 2;
			}
			else
			{
				dimensions = 1;
			}
		}

		internal override string GetString(CallingContext exec)
		{
			throw new ErrorException(ErrorCodes.Syntax);
		}

		internal override Variant GetVariant(CallingContext exec)
		{
			throw new ErrorException(ErrorCodes.Syntax);
		}

		internal override string GetName(CallingContext exec)
		{
			return name;
		}

		internal int Index
		{
			get { return indexValue; }
		}

		internal ArrayMember CreateMember(CallingContext ctx)
		{
			switch (dimensions)
			{
				case 1:
					return new ArrayMember(dimension1.GetInt(ctx));
				case 2:
					return new ArrayMember(dimension1.GetInt(ctx), dimension2.GetInt(ctx));
				default:
					throw new ErrorException(ErrorCodes.Syntax);
			}
		}

		internal override bool HasQualifier()
		{
			return qualifier != null;
		}

		private MemberResolver GetResolver(CallingContext context)
		{
			var expressionQualifier = qualifier as IExpressionQualifier;
			if (expressionQualifier == null)
				return context.Resolver;

			MemberResolver memberResolver = expressionQualifier.GetResolver(context);
			if (memberResolver == null)
				throw new ErrorException(ErrorCodes.UnknownMember, qualifier.GetName(context));
			return memberResolver;
		}

		internal override ValueMember GetValueMember(CallingContext context)
		{
			var v1 = GetArrayMember(context);
			if (v1 == null)
				throw new ErrorException(ErrorCodes.VariableNotFound);

			return Locate(context, v1);
		}

		public ArrayMember GetArrayMember(CallingContext context)
		{
			MemberResolver memberResolver;
			if (qualifier == null)
			{
				if (resolver == null)
					resolver = new VariableResolver();
				memberResolver = resolver;
			}
			else
				memberResolver = GetResolver(context);
			return  memberResolver.Resolve(context, new Nti(name)) as ArrayMember;
		}

		ValueMember Locate(CallingContext context, ArrayMember v1)
		{
			ValueMember val;
			switch (dimensions)
			{
				case 0:
					throw new ErrorException(ErrorCodes.Syntax);
				case 1:
					val = v1.Locate(1, dimension1.GetInt(context));
					break;
				case 2:
					val = v1.Locate(dimension1.GetInt(context), dimension2.GetInt(context));
					break;
				default:
					throw new ErrorException(ErrorCodes.InvalidSubscript);
			}
			return val;
		}

		public override string ToString()
		{
			switch (dimensions)
			{
				case 1:
					return name + "[" + dimension1 + "]";
				case 2:
					return name + "[" + dimension1 + "," + dimension2 + "]";
			}
			throw new ErrorException(ErrorCodes.InvalidSubscript);
		}

		public void ResizeMember(ArrayMember mbr, CallingContext ctx)
		{
			switch (dimensions)
			{
				case 1:
					mbr.Dimension(dimension1.GetInt(ctx));
					break;
				case 2:
					mbr.Dimension(dimension1.GetInt(ctx), dimension2.GetInt(ctx));
					break;
				default:
					throw new ErrorException(ErrorCodes.Syntax);
			}
		}
	}


	class Parenthesis : ExpressionBase
	{

		ExpressionBase value;

		override internal void Compile(Compiler comp)
		{
			value = comp.Stack.Pop();
		}

		internal override string GetName(CallingContext context)
		{
			// Parenthesis are used for name expressions. If the content evaluates
			// to a string, that is used as the name.
			// TODO: Check what happens when we use a non-existing variable name in parenthesis
			var name = value.GetVariant(context);
			if (name.Type == VariantType.Character)
				return name;
			return null;
		}

		internal override Variant GetVariant(CallingContext exec)
		{
			return value.GetVariant(exec);
		}

		internal override bool GetBool(CallingContext exec)
		{
			return value.GetBool(exec);
		}

		internal override int GetInt(CallingContext context)
		{
			return value.GetInt(context);
		}

		internal override string GetString(CallingContext context)
		{
			return value.GetString(context);
		}

		internal override int GetStringLength(CallingContext context)
		{
			return value.GetStringLength(context);
		}

		public override string ToString()
		{
			return "(" + value + ")";
		}
	}

	class DateConstant : ExpressionBase
	{
		Variant value;
		DateTime dateTime;
		double doubleValue;

		override internal void Compile(Compiler comp)
		{
			doubleValue = comp.Reader.ReadDouble();
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			if (doubleValue == 0)
				dateTime = new DateTime(0);
			else
			{
				dateTime = new DateTime(1900, 1, 1).AddDays(doubleValue - 2415021);
			}
			value = new Variant(dateTime, VariantType.Date);
			return value;
		}

		public override string ToString()
		{
			return "{^" + dateTime.Year + "/" + dateTime.Month + "/" + dateTime.Day + "}";
		}
	}

	class DateTimeConstant : ExpressionBase
	{
		Variant value;
		DateTime dateTime;

		override internal void Compile(Compiler comp)
		{
			var doubleValue = comp.Reader.ReadDouble();
			if (doubleValue == 0)
				dateTime = new DateTime(0);  // empty date
			else
			{
				dateTime = new DateTime(1900, 1, 1).AddDays(doubleValue - 2415021);
			}
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			value = new Variant(dateTime, VariantType.DateTime);
			return value;
		}

		public override string ToString()
		{
			return "{^" + dateTime.Year + "/" + dateTime.Month + "/" + dateTime.Day +
				" " + dateTime.Hour + ":" + dateTime.Minute + ":" + dateTime.Second + "}";
		}
	}

	class NumberConstant : ExpressionBase
	{
		double doubleValue;
		int length;
		int decimals;

		override internal void Compile(Compiler comp)
		{
			FixedInt = true;
			length = comp.Reader.ReadByte();
			decimals = comp.Reader.ReadByte();
			doubleValue = comp.Reader.ReadDouble();
		}

		internal override int GetInt(CallingContext exec)
		{
			return (int)doubleValue;
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			return new Variant(doubleValue, length, decimals);
		}

		internal override double GetDouble(CallingContext exec)
		{
			return doubleValue;
		}

		internal override bool GetBool(CallingContext exec)
		{
			return (doubleValue != 0.0);
		}

		public override string ToString()
		{
			// TODO: Use always a decimal point
			if (decimals == 0)
				return String.Format("{0:0}", doubleValue);

			return String.Format("{0:0." + "".PadRight(decimals, '0') + "}", doubleValue);
		}
	}

	class WorkArea : ExpressionBase, IExpressionQualifier
	{
		MemberResolver resolver;
		Byte workarea;

		internal override void Compile(Compiler er)
		{
			workarea = er.Reader.ReadByte();

			if (workarea == 0x0D)
			{
				resolver = new VariableResolver();
			}
		}

		internal override Variant GetVariant(CallingContext context)
		{
			return new Variant(false);
		}

		#region IExpressionQualifier Members

		public MemberResolver GetResolver(CallingContext context)
		{
			if (resolver == null)
			{
				if (workarea >= 0x01 && workarea <= 0x0A)
				{
					if (context.DataSession[workarea] != null)
						return new MemberListResolver(context.DataSession[workarea].Fields);
					return null;
				}
				return context.Resolver;
			}
			return resolver;
		}

		#endregion
		public override String ToString()
		{
			return new string(GuineuInstance.CurrentCp.GetChars(new[] { (byte)(64 + workarea) }));
		}
	}

	class Alias : ExpressionBase, IExpressionQualifier
	{
		Nti name;
		int index;
		ExpressionBase qualifier;

		override internal void Compile(Compiler comp)
		{
			if (comp.Stack.Count >= 1)
			{
				ExpressionBase testExp = comp.Stack.Peek();
				if (testExp is IExpressionQualifier)
				{
					qualifier = comp.Stack.Pop();
				}
			}

			index = comp.Reader.ReadInt16();
			name = comp.Code.GetNti(index);
		}

		public MemberResolver GetResolver(CallingContext context)
		{
			MemberResolver resolver = null;

			// check if Name is an alias
			Int32 area = context.DataSession.Select(name);
			if (area != 0)
			{
				return new MemberListResolver(context.DataSession[area].Fields);
			}

			// Resolve references such as THIS, THISFORM, etc.
			ObjectBase objRef = null;
			switch (name.ToKnownNti())
			{
				case KnownNti.This:
					objRef = context.This;
					break;
				case KnownNti.ThisForm:
					if (context.This != null)
					{
						objRef = context.This.GetThisForm();
					}
					break;
				case KnownNti.ThisFormSet:
					if (context.This != null)
					{
						objRef = context.This.GetThisForm();
					}
					break;
			}

			// If there's another qualifier preceding this qualifier, we have a multi-level
			// object reference.
			var expressionQualifier = qualifier as IExpressionQualifier;
			if (objRef != null)
			{
				resolver = new MemberListResolver(objRef);
			}
			else
			{
				MemberResolver objResolver;
				if (expressionQualifier == null)
					objResolver = new VariableResolver();
				else
					objResolver = expressionQualifier.GetResolver(context);

				// Name can be an object reference
				Member reference = objResolver.Resolve(context, name);
				if (reference != null)
				{
					var val = reference as ValueMember;
					if (val == null)
						throw new ErrorException(ErrorCodes.InvalidSubscript);

					if (val.Type == VariantType.Object)
					{
						objRef = val.Get();
						resolver = new MemberListResolver(objRef);
					}
					else
						throw new ErrorException(ErrorCodes.AliasNotFound, name.ToString());
				}
			}

			return resolver;
		}
		public ObjectBase GetResolverObject(CallingContext context)
		{
			// Resolve references such as THIS, THISFORM, etc.
			ObjectBase objRef = null;
			switch (name.ToKnownNti())
			{
				case KnownNti.This:
					objRef = context.This;
					break;
				case KnownNti.ThisForm:
					if (context.This != null)
					{
						objRef = context.This.GetThisForm();
					}
					break;
				case KnownNti.ThisFormSet:
					if (context.This != null)
					{
						objRef = context.This.GetThisForm();
					}
					break;
			}

			// If there's another qualifier preceding this qualifier, we have a multi-level
			// object reference.
			var expressionQualifier = qualifier as IExpressionQualifier;
			if (objRef != null)
				return objRef;

			MemberResolver objResolver;
			if (expressionQualifier == null)
				objResolver = new VariableResolver();
			else
				objResolver = expressionQualifier.GetResolver(context);

			// Name can be an object reference
			Member reference = objResolver.Resolve(context, name);
			if (reference != null)
			{
				var val = reference as ValueMember;
				if (val == null)
					throw new ErrorException(ErrorCodes.InvalidSubscript);

				if (val.Type == VariantType.Object)
					return val.Get();
			}

			return null;
		}

		internal override Variant GetVariant(CallingContext context)
		{
			return new Variant(true);
		}
		/// <summary>
		/// Returns the cursor if the qualifier is an alias Name.
		/// </summary>
		/// <param name="exec"></param>
		/// <returns>ICursor for the alias, or null if no alias is specified</returns>
		internal override ICursor GetCursor(CallingContext exec)
		{
			if (HasQualifier())
				return null;
			if (exec.DataSession.Select(name) == 0)
				return null;
			return exec.DataSession.GetExistingCursor(name);
		}
		internal override bool HasQualifier()
		{
			return qualifier != null;
		}

		public override string ToString()
		{
			String s = "";
			if (qualifier != null)
				s = qualifier + ".";
			return s + name;
		}
	}

	class ConstantFalse : ExpressionBase
	{
		override internal void Compile(Compiler comp) { }
		override internal Variant GetVariant(CallingContext exec)
		{
			return new Variant(false);
		}
		public override string ToString()
		{
			return ".F.";
		}
	}

	class ConstantNull : ExpressionBase
	{
		override internal void Compile(Compiler comp) { }

		override internal Variant GetVariant(CallingContext exec)
		{
			return new Variant(VariantType.Logical, true);
		}
		public override string ToString()
		{
			return ".NULL.";
		}
	}

	/// <summary>
	/// Returns NULL in ICASE
	/// </summary>
	/// <remarks>
	/// This token is used in ICASE() at the end of the parameter list when there's no
	/// default value at the end of the parameter list, eg. ICASE(1=1,2,3=3,4). So far
	/// I've only seen the combination FF 02 which results in NULL being returned. If
	/// the token 
	/// </remarks>
	class ReturnDefault : ExpressionBase
	{
		Byte what;

		override internal void Compile(Compiler comp)
		{
			what = comp.Reader.ReadByte();
			Debug.Assert(what == 2);
		}

		override internal Variant GetVariant(CallingContext exec)
		{
			return new Variant(VariantType.Logical, true);
		}

		public override string ToString()
		{
			return "";
		}
	}


	class ConstantTrue : ExpressionBase
	{
		override internal void Compile(Compiler comp)
		{
		}
		override internal Variant GetVariant(CallingContext exec)
		{
			return new Variant(true);
		}
		public override string ToString()
		{
			return ".T.";
		}
	}

	class WriteArray : ExpressionBase
	{
		ExpressionBase array;

		internal override void Compile(Compiler c)
		{
			array = c.GetCompiledExpression();
		}

		internal override Variant GetVariant(CallingContext context)
		{
			// shouldn't be called
			// TODO: throw exception
			return new Variant(array.GetName(context));
		}

		internal ArrayMember GetArray(CallingContext context)
		{
			var locals = context.Locals;
			var name = array.ToNti(context);
			var mbr = context.Resolver.Resolve(context, name);

			// array doesn't exist
			ArrayMember retVal;
			if (mbr == null)
			{
				retVal = new ArrayMember(1);
				locals.Add(name, retVal);
			}
			else
			{
				retVal = mbr as ArrayMember;
			}
			return retVal;
		}

		public override string ToString()
		{
			return array.ToString();
		}
	}

	class ReadArray : ExpressionBase
	{
		ExpressionBase array;

		internal override void Compile(Compiler c)
		{
			array = c.GetCompiledExpression();
		}

		internal override Variant GetVariant(CallingContext context)
		{
			return new Variant(array.GetName(context));
		}

		internal ArrayMember GetArray(CallingContext context)
		{
			var name = array.ToNti(context);
			Member mbr = context.Resolver.Resolve(context, name);
			if (mbr == null)
				throw new ErrorException(ErrorCodes.VariableNotFound, name.ToString());
			return mbr as ArrayMember;
		}

		public override string ToString()
		{
			return array.ToString();
		}
	}

	class ArrayMemberAccess : ExpressionBase
	{
		Nti name;
		ExpressionBase qualifier;

		internal override void Compile(Compiler c)
		{
			// ArrayMemberAccess (0xE0) always belongs to an object
			qualifier = c.Stack.Pop();
			var index = c.Reader.ReadInt16();
			name = c.Code.GetNti(index);
		}

		internal override Variant GetVariant(CallingContext context)
		{
			return new Variant(name);
		}

		internal ArrayMember GetArray(CallingContext context)
		{
			var r = GetResolver(context);
			Member mbr = r.Resolve(context, name);
			if (mbr == null)
				throw new ErrorException(ErrorCodes.VariableNotFound, name.ToString());
			return mbr as ArrayMember;
		}

		public override string ToString()
		{
			return name.ToString();
		}

		internal override bool HasQualifier()
		{
			return qualifier != null;
		}

		private MemberResolver GetResolver(CallingContext context)
		{
			var expressionQualifier = qualifier as IExpressionQualifier;
			if (expressionQualifier == null)
				return context.Resolver;

			MemberResolver memberResolver = expressionQualifier.GetResolver(context);
			if (memberResolver == null)
				throw new ErrorException(ErrorCodes.UnknownMember, qualifier.GetName(context));
			return memberResolver;
		}
	}

	class SkipOnTrue : ExpressionBase
	{
		ExpressionBase op;

		override internal void Compile(Compiler comp)
		{
			comp.Reader.ReadInt16();
			op = comp.Stack.Pop();
		}

		override internal Variant GetVariant(CallingContext context)
		{
			bool result = GetBool(context);
			var retVal = new Variant(result);
			return retVal;
		}

		internal override bool GetBool(CallingContext context)
		{
			return op.GetBool(context);
		}

		public override string ToString()
		{
			return op.ToString();
		}
	}

	class SkipOnFalse : ExpressionBase
	{
		ExpressionBase op;

		override internal void Compile(Compiler comp)
		{
			comp.Reader.ReadInt16();
			op = comp.Stack.Pop();
		}

		override internal Variant GetVariant(CallingContext context)
		{
			bool result = GetBool(context);
			var retVal = new Variant(result);
			return retVal;
		}

		internal override bool GetBool(CallingContext context)
		{
			return op.GetBool(context);
		}

		public override string ToString()
		{
			return op.ToString();
		}
	}

	class ByVal : ExpressionBase
	{
		ExpressionBase value;

		internal override void Compile(Compiler c)
		{
			value = c.GetCompiledExpression();
		}

		internal override Variant GetVariant(CallingContext context)
		{
			return value.GetVariant(context);
		}
		internal override ValueMember GetValueMember(CallingContext context)
		{
			return value.GetValueMember(context);
		}
		internal override string GetName(CallingContext context)
		{
			return value.GetName(context);
		}

		public override ValueMember GetParameterValue(CallingContext ctx)
		{
			return new ValueMember(value.GetVariant(ctx));
		}

		public override string ToString()
		{
			return value.ToString();
		}
	}

	class ByRef : ExpressionBase
	{
		ExpressionBase value;

		internal override void Compile(Compiler c)
		{
			value = c.GetCompiledExpression();
		}

		internal override Variant GetVariant(CallingContext context)
		{
			return value.GetVariant(context);
		}
		internal override ValueMember GetValueMember(CallingContext context)
		{
			return value.GetValueMember(context);
		}
		internal override string GetName(CallingContext context)
		{
			return value.GetName(context);
		}

		public override ValueMember GetParameterValue(CallingContext ctx)
		{
			return value.GetValueMember(ctx);
		}

		public override string ToString()
		{
			return "@" + value;
		}
	}
}