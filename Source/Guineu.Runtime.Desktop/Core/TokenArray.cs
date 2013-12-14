using System;

namespace Guineu.Core
{
	public class TokenArray<TToken, TInterface, TSource>
		where TToken : struct
		where TInterface : class
		where TSource : class
	{
		readonly Func<TSource, TInterface>[] list = new Func<TSource, TInterface>[255];
		readonly ErrorCodes tokenError;
		readonly String errMsg;
		readonly Int32 startIndex;

		public TokenArray(ErrorCodes error, String msg, Int32 index)
		{
			tokenError = error;
			errMsg = msg;
			startIndex = index;
		}

		public TokenArray(ErrorCodes error)
			: this(error, "", 0)
		{ }

        public TokenArray()
            : this(0, "", 0)
        { }
        
        public void Add<TClass>(TToken token) where TClass : TInterface, new()
		{
			list[Index(token)] = (TSource source) => new TClass();
		}

		Int32 Index(TToken token)
		{
			var index = (Int32) (ValueType) token;
			return index - startIndex;
		}

			public TInterface Get(TToken token, TSource source)
		{
					var index = Index(token);
				if (list[index] == null)
				{
                    if (tokenError != 0 && !GuineuInstance.IgnoreUnknownTokens)
						throw new ErrorException(
							tokenError,
							errMsg + token + " (" + ( (Int32) (ValueType) token).ToString("x") + ")"
							);
					return null;
				}
				return list[index](source);
		}

		public void Add(TToken token, Func<TSource, TInterface> func)
		{
			list[Index(token)] = func;
		}
	}
}
