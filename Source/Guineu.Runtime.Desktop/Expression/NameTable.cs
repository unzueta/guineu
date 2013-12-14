using System;
using System.Collections.Generic;
using System.Text;

namespace Guineu.Expression
{
	class NameTable
	{
		SortedDictionary<String, NameTableItem> list;

		public NameTable()
		{
			list = new SortedDictionary<String, NameTableItem>();
		}

		public NameTableItem GetNti(String name)
		{
			NameTableItem nti = new NameTableItem(name);
			if (list.ContainsKey(nti.Key))
				return list[nti.Key];
			else
			{
				list.Add(nti.Key, nti);
				return nti;
			}
		}
	}

	class NameTableItem
	{
		String name;

		public NameTableItem(String theName)
		{
			this.name = theName.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
			if (this.name != null)
				if (this.name.Length >= 2)
				{
					if (this.name[0] == '"' && this.name[this.name.Length - 1] == '"')
						this.name = this.name.Substring(1, this.name.Length - 2);
					else if (this.name[0] == '\'' && this.name[this.name.Length - 1] == '\'')
						this.name = this.name.Substring(1, this.name.Length - 2);
				}
		}

		public override int GetHashCode()
		{
			return name.GetHashCode();
		}
		public bool Equals(String compareTo)
		{
			return (name == compareTo);
		}

		public String Key
		{
			get { return name; }
		}
	}
}
