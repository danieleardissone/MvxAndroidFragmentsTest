using System;
using System.Diagnostics;

namespace MvxAndroidFragmentsTest.Core.Exceptions
{
	public class MvxAndroidFragmentsTestOfflineException : Exception
	{
		publicMvxAndroidFragmentsTestOfflineException() : base()
		{
		}

		public MvxAndroidFragmentsTestOfflineException(string message) : base (message)
		{
			Debug.WriteLine (message);
		}

		public MvxAndroidFragmentsTestOfflineException(string message, Exception innerException) : base (message, innerException)
		{
			Debug.WriteLine (message + Environment.NewLine + innerException);
		}
	}
}
