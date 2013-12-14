using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using Guineu.Expression;

namespace Guineu
{
    /// <summary>
    /// Default runtime file to execute any external FXP file.
    /// </summary>
    class main
    {
        static void Main4(string[] args)
        {
            var ex = Tokenizer.ParseExpression("lcName.Alias.dsafsadf.dsafasdf");
            ex.Stream.Seek(0, SeekOrigin.Begin);

            Int32 cnt = 0;
            int b;
            do
            {
                if (cnt % 16 == 0)
                    Console.Write(String.Format("{0:X4}: ", cnt));

                b = ex.Stream.ReadByte();
                if (b < 0)
                    break;
                Console.Write(String.Format("{0:X2} ", b));
                cnt++;
                if (cnt % 16 == 0)
                    Console.WriteLine();
            } while (true);

            Console.WriteLine();
            Console.WriteLine();
            for (Int32 i = 0; i < ex.Names.Count; i++)
            {
                Console.Write(String.Format("{0:X4}: {1}", i, ex.Names[i]));
            }

            Console.ReadKey();
        }

        //static void Main2(string[] args)
        //{
        //    Sample x = new Guineu.Data.Dbf2.Sample();
        //    x.Execute();
        //}
        static void Main2(string[] args)
        {
            var nv = new NewVariant(5, 10);
            var v = new Variant(5, 10);
            Object o = 5;

            RunTest("Base", () => { var x = false; });
            //			RunTest("Variant, DateTime", () => { var x = new Variant(DateTime.Now); });
            RunTest("Variant, String", () => { var x = new Variant("Hallo"); });
            RunTest("Variant, Int32", () => { var x = new Variant(5, 10); });
            RunTest("NewVariant, Int32", () => { var x = new NewVariant(5, 10); });
            RunTest("NewVariant, CreateInt32", () => { var x = NewVariant.CreateInt32(5); });
            RunTest("NewVariant, String", () => { var x = new NewVariant("Hallo"); });
            RunTest("NewVariant, Conversion", () => { Int32 x = nv; });
            RunTest("Variant, Conversion", () => { Int32 x = v; });
            RunTest("Object, Int32", () => { Object x = 5; });
            RunTest("Int32", () => { Int32 x = 5; });
            RunTest("Object, Conversion", () => { Int32 x = (Int32)o; });
            Console.ReadKey();
        }

        static void RunTest(String bez, Action x)
        {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 10000000; i++)
            {
                x();
            }
            double seconds = sw.ElapsedTicks / 1000;
            Console.WriteLine(String.Format("{0}: {1}", bez, seconds));
        }

        static void Main(string[] args)
        {
            // Initialize Guineu
            Application.EnableVisualStyles();
            GuineuInstance.InitInstance();

            // Determine which FXP file Guineu should execute. This is either the first parameter
            // or a file that has the same name as the EXE.
            String filename = GetFileName(args);

            String[] param = null;
            if (args.Length >= 0)
            {
                param = new string[args.Length - 1];
                for (var i = 1; i < args.Length; i++)
                    param[i - 1] = args[i];
            }

            // execute the FXP file
            if (filename != null)
                GuineuInstance.Do(filename, param);
        }

        static String GetFileName(string[] args)
        {
            String filename;
            if (args.Length == 0)
                filename = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "FXP");
            else filename = Path.GetExtension(args[0]) == String.Empty ? Path.ChangeExtension(args[0], "FXP") : args[0];
            if (!GuineuInstance.FileMgr.Exists(filename))
            {
                var dlg = new OpenFileDialog
                              {
                                  Filter = @"Executables (*.fxp)|*.fxp;*.FXP|All files (*.*)|*.*"
                              };
                if (dlg.ShowDialog() == DialogResult.OK)
                    filename = dlg.FileName;
                else
                    filename = null;
            }
            return filename;
        }
    }
}