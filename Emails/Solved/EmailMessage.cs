using System.Text;

namespace Emails.Solved
{
	public class EmailMessage
	{
		public bool Important;
		public string Subject;
		public StringBuilder Body = new StringBuilder();
	}
}