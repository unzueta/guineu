namespace System
{
    public delegate TResult Func<T, TResult>(T arg);
    public delegate TResult Func<T1, T2, TResult>(T1 arg, T2 arg2);
    public delegate TResult Func<TResult>();

	public delegate void Action();
	// public delegate void Action<T>(T arg1);


	namespace Runtime.CompilerServices
	{
		public class ExtensionAttribute : Attribute { }
	}
}