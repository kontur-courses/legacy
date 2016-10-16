using System.Text;

namespace Emails.Solved
{
	public class EmailMessage
	{
		public string Subject;
		public StringBuilder Body = new StringBuilder();
		public bool Important;
	}
}