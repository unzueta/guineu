using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Guineu.Expression
{
	public class CompiledExpression
	{
		readonly MemoryStream streamField = new MemoryStream(0);
		readonly List<String> namesField = new List<string>();

		public MemoryStream Stream { get { return streamField; } }
		public List<String> Names { get { return namesField; } }
	}

	public static class Tokenizer
	{
		internal static CodeBlock Expression(String expr)
		{
			var expression = ParseExpression(expr);

			var cl = new CodeLine();
			expression.Stream.SetLength(expression.Stream.Position);
			cl.Line = expression.Stream.ToArray();
			var code = new List<CodeLine> { cl };
			var cb = new CodeBlock(new CompiledCode(code, expression.Names), null, null);
			cb.Goto(0);
			return cb;
		}

		public static CompiledExpression ParseExpression(string expr)
		{
			var exp = new CompiledExpression();
			var bin = new BinaryWriter(exp.Stream);

			var rd = new StringReader(expr);

			for (Int32 ch = 0; ch < expr.Length; ch++)
			{
				// "..." '...' [...]
				// fct(...) fct[...]
				// (...)
				// abcd
				// 1234.45e3
				//
				// abcd.  abcd(  abcd[  abcd->

				if (char.IsLetter(expr[ch]) || expr[ch] == '_')
				{
					String name = ReadString(expr, ref ch);
					if (!exp.Names.Contains(name))
					{
						exp.Names.Add(name);
					}
					if (ch >= expr.Length)
					{
						bin.Write((byte)Token.Variable);
						bin.Write((Int16)exp.Names.IndexOf(name));
					}
					else
					{
						if (expr[ch] == '.')
						{
							bin.Write((byte)Token.Alias);
							bin.Write((Int16)exp.Names.IndexOf(name));
						}
						else
						{
							bin.Write((byte)Token.Variable);
							bin.Write((Int16)exp.Names.IndexOf(name));
						}
					}
					continue;
				}
			}
			return exp;
		}

		internal static CodeBlock Expression(Byte[] expr)
		{
			return Expression(new String(GuineuInstance.CurrentCp.GetChars(expr)));
		}

		private static string ReadString(string expr, ref int ch)
		{
			String ret = "";
			while (ch < expr.Length && (char.IsLetterOrDigit(expr[ch]) || expr[ch] == '_'))
			{
				ret += char.ToUpper(expr[ch], System.Globalization.CultureInfo.InvariantCulture);
				ch++;
			}
			return ret;
		}
	}

	public class NormalizedLine
	{
		readonly String code;

		public NormalizedLine(String line, CallingContext ctx)
		{
			code = Normalize(line, ctx);
		}

		enum TokenType
		{
			Character,
			Other
		}
		static string Normalize(string line, CallingContext ctx)
		{
			var sb = new StringBuilder();

			Int32 pos = 0;
			TokenType last = TokenType.Other;
			while (pos < line.Length)
			{
				char ch = line[pos];
				switch (ch)
				{
					// Ignore any blanks in expressions, but not between clauses and within literals
					case ' ':
					case '\t':
						if (last == TokenType.Character)
							if (pos + 1 < line.Length)
								if (IsPartOfClause(line[pos + 1]))
									sb.Append(' ');
						break;

					// Literals remain unchanged
					case '\'':
					case '"':
					case '[':
						sb.Append(ch);
						while (pos + 1 < line.Length)
						{
							pos++;
							sb.Append(line[pos]);
							if (line[pos] == ch)
								break;
						}
						last = TokenType.Other;
						break;

					// Macros are only substituted when a context is passed in. This happens on 
					// the first level and at runtime only. Compiler passes and nested substitutions
					// do not pass a context.
					case '&':
						if (ctx == null)
							sb.Append(ch);
						else
						{
							var name = "";
							while (pos + 1 < line.Length)
							{
								ch = line[pos + 1];
								if (Char.IsLetter(ch))
								{
									name = name + ch;
									pos++;
								}
								else
								{
									if (ch == '.')
										pos++;
									break;
								}
							}
							if (!String.IsNullOrEmpty(name))
							{
								var resolver = new VariableResolver();
								var variable = resolver.Resolve(ctx, new Nti(name));
								var val = variable as ValueMember;
								if (val != null)
								{
									var macro = Normalize(val.Get(), null);
									sb.Append(macro);
								}
							}
						}
						break;

					// Code is always uppercase
					default:
						String s = ch.ToString().ToUpper(System.Globalization.CultureInfo.InvariantCulture);
						if (Char.IsLetter(ch))
							last = TokenType.Character;
						else
							last = TokenType.Other;
						sb.Append(s);
						break;
				}
				pos++;
			}
			return sb.ToString();
		}

		static Boolean IsPartOfClause(Char ch)
		{
			return Char.IsLetter(ch) || ch == '&';
		}

		public String Code { get { return code; } }
	}

	public class GuineuCompiler
	{
		// TODO: Maintain a name list for this compiler (keep adding for every line)

		


		public Byte[] Compile(NormalizedLine line)
		{
			String code = line.Code;

			CommandTokens token;
			var pos = code.IndexOf(' ');
			if (pos == 0)
				token = CommandTokens.LET;
			else
			{
				String cmd = code.Substring(0, pos);
				// TODO: Check whether cmd is a true command, or whether it's an expression fragment to avoid try
				// TODO: Check minimum length and maximum length for commands (BROW, BROWS, BROWSE)
				try
				{
					token = (CommandTokens)Enum.Parse(typeof(CommandTokens),cmd,true);
				}
				catch (Exception)
				{
					token = CommandTokens.LET;
				}
			}
			return Compile(token);
		}

		static byte[] Compile(CommandTokens token)
		{
			using (var stream = new MemoryStream())
			{
				using (var bw = new BinaryWriter(stream))
				{
					bw.Write((Byte) token);
					// TODO: compile remaining part
				}

				//var language = new
				//                {
				//                  {CommandTokens.SET, Token.FILTER, Token.TO, {null, typeof(ExpressionBase)}},
				//{CommandTokens.USE}
				//                };

				return stream.ToArray();
			}
		}
	}

}