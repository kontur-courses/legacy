using System.Text;

namespace Emails
{
	public class EmailMessage
	{
	    public bool Important;
	    public string Subject;
	    public StringBuilder Body = new StringBuilder();
	}
}