using System.Text;

namespace Emails
{
	public class EmailMessage
	{
		public string Subject;
		public StringBuilder Body = new StringBuilder();
		public bool Important;
	}
}