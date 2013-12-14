namespace Guineu.Data
{
	public class SptConnectionManager : IndexedList<SptConnection>
	{
		public override SptConnection this[int handle]
		{
			get
			{
				if (!IsValid(handle))
				{
					throw new ErrorException(ErrorCodes.ConnectionHandleInvalid);
				}
				return base[handle];
			}
			set
			{
				base[handle] = value;
			}
		}

		private ISptEngine _Engine;

		public ISptEngine Engine
		{
			get { return _Engine; }
			set { _Engine = value; }
		}
	}
}
