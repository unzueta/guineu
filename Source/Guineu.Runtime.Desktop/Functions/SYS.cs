using System;
using System.Collections.Generic;
using Guineu.Expression;

namespace Guineu.Functions
{
	partial class SYS : ExpressionBase
	{
		List<ExpressionBase> param;
        static readonly Factory List = new Factory();

        static SYS()
        {
            List.Register<SYS1>(1);
            List.Register<SYS2>(2);
            List.Register<SYS3>(3);
            List.Register<SYS16>(16);
            List.Register<SYS1079>(1079);
            List.Register<SYS2000>(2000);
            List.Register<SYS2015>(2015);
            List.Register<SYS8000>(8000);
            List.Register<SYS8001>(8001);
            List.Register<SYS8002>(8002);
            List.Register<SYS8003>(8003);
            List.Register<SYS8006>(8006);
            List.Register<SYS8007>(8007);
            List.Register<SYS8008>(8008);
            List.Register<SYS8011>(8011);
            List.Register<SYS8012>(8012);
            List.Register<SYS8013>(8013);
            List.Register<SYS8014>(8014);
            List.Register<SYS8015>(8015);
            RegisterFunctions();

        }

		override internal void Compile(Compiler comp)
		{
			param = comp.GetParameterList();
			if (param.Count == 0)
			{
				throw new ErrorException(ErrorCodes.TooFewArguments);
			}
		}

		internal override Variant GetVariant(CallingContext context)
		{
			return new Variant(GetString(context));
		}

		internal override string GetString(CallingContext context)
		{
			var function = param[0].GetInt(context);
		    var func = List.Get(function);
		    string retVal = func.getString(context, param);
			return retVal;
		}

        class Factory
        {
            readonly Dictionary<Int32, Func<ISys>> list;

            public Factory()
            {
                list = new Dictionary<Int32, Func<ISys>>();
            }

            public ISys Get(Int32 cmd)
            {
                try
                {
                    return list[cmd]();
                }
                catch (KeyNotFoundException)
                {
                    return new SysDefault();
                }
            }

            public void Register<TClass>(Int32 cmd) where TClass : ISys, new()
            {
                Register(cmd, () => new TClass());
            }
            void Register(Int32 cmd, Func<ISys> fnc)
            {
                list.Add(cmd, fnc);
            }
        }
    }

    interface ISys
    {
        string getString(CallingContext context, List<ExpressionBase> param);
    }

    class SysDefault : ISys
    {
        public string getString(CallingContext context, List<ExpressionBase> param)
        {
            return "";
        }
    }
}