using System;
using Guineu.Expression;
using Guineu.Gui;

namespace Guineu.ObjectEngine
{
	public abstract class NestedClass : ObjectBase
	{
		ParentProperty parent;

		protected NestedClass(ObjectTemplate obj)
			: base(obj)
		{
			AddParentProperty();
		}

		/// <summary>
		/// Every Guineu class relies on a platform specific implementation
		/// </summary>
		public IControl View { get; protected set; }

		void AddParentProperty()
		{
			parent = new ParentProperty();
			AddMember(parent);
			parent.AssignParent(this);
		}
	}

	internal abstract partial class NestedClassTemplate : ObjectTemplate
	{
		internal NestedClassTemplate() { }
		internal NestedClassTemplate(String name) : base(name) { }

		protected override void DoAddMembers()
		{
			AddProperty(KnownNti.Parent, 0);
		}

		protected override void DoAddMembers(IMemberList template)
		{
			CloneMember(KnownNti.Parent, template);
		}
	}
}