using System;
using Guineu.Expression;
using Guineu.Gui;
using Guineu.ObjectEngine;

namespace Guineu.Classes
{
	class TimerClass : NestedClass
	{
		readonly IControl timer;

		readonly GenericEvent timerEvent;
		readonly GenericProperty enabledProperty;
		readonly GenericProperty intervalProperty;

		internal TimerClass(ObjectTemplate obj)
			: base(obj)
		{
			timer = GuineuInstance.WinMgr.CreateControl(KnownNti.Timer);

			enabledProperty = new GenericProperty(KnownNti.Enabled, GetPropVariant(KnownNti.Enabled));
			intervalProperty = new GenericProperty(KnownNti.Interval, GetPropVariant(KnownNti.Interval));
			timerEvent = new GenericEvent(this, KnownNti.Timer);
		}

		protected override void DoInitializeInstance()
		{
			base.DoInitializeInstance();
			AddMembers();
			BindMembers();
		}

		void AddMembers()
		{
			AddMember(enabledProperty);
			AddMember(intervalProperty);
			Add(KnownNti.Timer, timerEvent);
			AddUserDefinedMembers();
		}

		void BindMembers()
		{
			intervalProperty.AssignParent(timer);
			enabledProperty.AssignParent(timer);
			timer.EventHandler += NotifyEvent;
		}

		void NotifyEvent(EventData e)
		{
			switch (e.Event)
			{
				case KnownNti.Timer:
					using (var ctx = new CallingContext(GuineuInstance.Context, this))
						timerEvent.Execute(ctx, e.Parameters);
					break;
			}
		}
	}

	class TimerClassTemplate : NestedClassTemplate
	{
		internal TimerClassTemplate() { }
		internal TimerClassTemplate(String name) : base(name) { }

		protected override ObjectBase DoCreateInstance()
		{
			return new TimerClass(this);
		}

		protected override void DoAddMembers()
		{
			AddProperty(KnownNti.Enabled, true);
			AddProperty(KnownNti.Interval, 0);
		}

		protected override void DoAddMembers(IMemberList template)
		{
			CloneMember(KnownNti.Enabled, template);
			CloneMember(KnownNti.Interval, template);
		}

		protected override ObjectTemplate DoCreateTemplate()
		{
			return new TimerClassTemplate();
		}
	}
}