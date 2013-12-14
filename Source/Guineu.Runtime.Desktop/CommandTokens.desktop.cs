using Guineu.Commands;
using Guineu.Expression;

namespace Guineu
{
	partial class TokenCompiler
	{
		partial void FillCommands()
		{
			cmd.Register<CLEAREVENTS>(CommandTokens.CLEAR, Token.EVENTS);
			cmd.Register<CLOSEDATABASES>(CommandTokens.CLOSE, Token.DATABASES);
			cmd.Register<COPYFILE>(CommandTokens.COPY, Token.FILE_Clause);
			cmd.Register<CREATECURSOR>(CommandTokens.CREATE, Token.CURSOR);
			cmd.Register<DELETEFILE>(CommandTokens.DELETE, Token.FILE_Clause);
			cmd.Register<READEVENTS>(CommandTokens.READ, Token.EVENTS);
			cmd.Register<SETFILTER>(CommandTokens.SET, SetToken.FILTER);
			cmd.Register<SETORDER>(CommandTokens.SET, SetToken.ORDER);
			cmd.Register<PRINT>(CommandTokens.PRINT);
			cmd.Register<PRINTPRINT>(CommandTokens.PRINTPRINT);
			cmd.Register<SCAN>(CommandTokens.SCAN);
			cmd.Register<USE>(CommandTokens.USE);
			cmd.Register<SELECT>(CommandTokens.SELECT);
			cmd.Register<APPEND>(CommandTokens.APPEND);
			cmd.Register<REPLACE>(CommandTokens.REPLACE);
			cmd.Register<SEEK>(CommandTokens.SEEK);
			cmd.Register<GOTO>(CommandTokens.GOTO);
			cmd.Register<SKIP>(CommandTokens.SKIP);
			cmd.Register<ZAP>(CommandTokens.ZAP);
			cmd.Register<RUN>(CommandTokens.RUN);
			cmd.Register<ERASE>(CommandTokens.ERASE);
			cmd.Register<WAIT>(CommandTokens.WAIT);
			cmd.Register<SCATTER>(CommandTokens.SCATTER);
			cmd.Register<GATHER>(CommandTokens.GATHER);
		}
	}
}
