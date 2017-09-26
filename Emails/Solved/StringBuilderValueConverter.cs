using System;
using System.Text;
using StatePrinting.ValueConverters;

namespace Emails.Solved
{
	public class StringBuilderValueConverter : IValueConverter
	{
		public bool CanHandleType(Type type)
		{
			return type == typeof(StringBuilder);
		}

		public string Convert(object source)
		{
			return ((StringBuilder) source).ToString();
		}
	}
}