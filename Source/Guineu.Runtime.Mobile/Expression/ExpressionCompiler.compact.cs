using Guineu.Functions;

namespace Guineu.Expression
{
	partial class Compiler
	{
		partial void FillFunctions()
		{
			Add<ASC>(Token.ASC);
			Add<SEEKFunction>(Token.SEEK);
			Add<APRINTERS>(Token.APRINTERS);
			Add<SYSMETRIC>(Token.SYSMETRIC);
			Add<ADIR>(Token.ADIR);
			Add<FONTMETRIC>(Token.FONTMETRIC);
			Add<SQLDISCONNECT>(Token.SQLDISCONNECT);
			Add<SQLEXEC>(Token.SQLEXEC);
			Add<SQLSTRINGCONNECT>(Token.SQLSTRINGCONNECT);
			Add<STRTOFILE>(Token.STRTOFILE);
			Add<MessageboxFunction>(Token.MESSAGEBOX);

           Add<FERROR>(Token.FERROR);
            Add<FCHSIZE>(Token.FCHSIZE);
            Add<FCREATE>(Token.FCREATE);
            Add<FOPEN>(Token.FOPEN);
            Add<FSEEK>(Token.FSEEK);
            Add<FGETS>(Token.FGETS);
            Add<FERROR>(Token.FERROR);
            Add<FREAD>(Token.FREAD);
            Add<FPUTS>(Token.FPUTS);
            Add<FWRITE>(Token.FWRITE);
            Add<FDATE>(Token.FDATE);
            Add<FTIME>(Token.FTIME);
            Add<FEOF>(Token.FEOF);
            Add<FFLUSH>(Token.FFLUSH);
            Add<FCLOSE>(Token.FCLOSE);
		}
	}
}

