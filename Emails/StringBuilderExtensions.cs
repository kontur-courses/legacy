using System.Text;

namespace Emails
{
    public static class StringBuilderExtensions
    {
        public static void AppendFormatLine(this StringBuilder sb, string format, params object[] values)
        {
            sb.AppendLine(string.Format(format, values));
        }
    }
}