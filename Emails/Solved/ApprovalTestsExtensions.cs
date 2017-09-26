using System;
using ApprovalTests;
using StatePrinting;

namespace Emails.Solved
{
	public static class ApprovalTestsExtensions
	{
		public static Lazy<Stateprinter> Stateprinter =
			new Lazy<Stateprinter>(() =>
			{
				var printer = new Stateprinter();
				printer.Configuration.Add(new StringBuilderValueConverter());
				return printer;
			});

		public static void Verify(this string value)
		{
			Approvals.Verify(value);
		}

		public static string Print<T>(this T value)
		{
			return Stateprinter.Value.PrintObject(value);
		}

		public static T ApplyTo<T>(this Config newConfig, Func<T> func)
		{
			var savedConfig = Config.Local;
			Config.Local = newConfig;
			try
			{
				return func();
			}
			finally
			{
				Config.Local = savedConfig;
			}
		}
	}
}