using System;

namespace MvxAndroidFragmentsTest.Core.Entities
{
	public class EventArgs<T> : EventArgs
	{
		public T Value { get; private set; }

		public EventArgs(T value) : base()
		{
			Value = value;
		}
	}
}
