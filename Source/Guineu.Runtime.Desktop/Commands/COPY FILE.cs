using System;
using System.IO;
using Guineu.Expression;

namespace Guineu.Commands
{
  class COPYFILE : ICommand
  {
    ExpressionBase SourceFile;
    ExpressionBase DestFile;

    public void Compile(CodeBlock code)
    {
      var comp = new Compiler(null, code);
      Token nextToken = code.Reader.PeekToken();
      do
      {
        switch (nextToken)
        {
          case Token.TO:
            code.Reader.ReadToken();
            DestFile = comp.GetCompiledExpression();
            break;

          default:
            SourceFile = comp.GetCompiledExpression();
            break;
        }
        nextToken = code.Reader.PeekToken();
      } while (nextToken != Token.CmdEnd);
    }

    public void Do(CallingContext context, ref Int32 nextLine)
    {
      String source = GuineuInstance.FileMgr.MakePath(SourceFile.GetString(context));
      String dest;
      if (DestFile == null)  // destination is optional
        dest = ".";
      else
        dest = DestFile.GetString(context);
    	dest = GuineuInstance.FileMgr.MakePath(dest);

      // TODO: Add warning when file exists.
      // TODO: Check if all formats are supported
      var dir = Path.GetDirectoryName(source);
      if (String.IsNullOrEmpty(dir))
        dir = GuineuInstance.FileMgr.CurrentDirectory;
			dir = GuineuInstance.FileMgr.MakePath(dir);

      var files = Directory.GetFiles(dir, Path.GetFileName(source));
      foreach (var n in files)
      {
        String dn = dest;
        String sourceName = Path.GetFileName(source);
        String destDir = Path.GetDirectoryName(dest);
        String destName = Path.GetFileName(dest);
        if (sourceName.IndexOf('*')>=0 && destName.IndexOf('*')>=0)
        {
          Int32 pos = sourceName.IndexOf('*');
          Int32 pos2 = destName.IndexOf('*');
          String remainer = Path.GetFileName(n).Substring(pos);
          dn = Path.Combine(destDir, destName.Substring(0, pos2) + remainer + destName.Substring(pos2 + 1))
            ;
        }
        File.Copy(n, dn, true);
      }

    }

		
  }
}