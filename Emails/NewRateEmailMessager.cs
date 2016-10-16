namespace Emails
{
	public class NewRateEmailMessager
	{
		public EmailMessage CreateMessage(string customerName, AccountType accountType, decimal newRate)
		{
			var message = new EmailMessage();

			message.Subject = "New rate!";
			var sb = message.Body;

			sb.AppendFormatLine("Dear {0}", customerName);
			sb.AppendLine();

			sb.Append("We are sending you this message with respect to your ");
			switch (accountType)
			{
				case AccountType.Cheque:
					sb.Append("chequing account");
					break;
				case AccountType.Savings:
					sb.Append("on line savings account");
					break;
				case AccountType.Credit:
					sb.Append("credit card");
					break;
			}

			sb.AppendLine();
			sb.AppendLine();


			if (Config.Local.IncreaseRate)
			{
				newRate *= Config.Local.IncreaseRateFactor;
			}
			switch (accountType)
			{
				case AccountType.Cheque:
					sb.AppendFormatLine("The interest rate at which you earn interest has changed to {0}%.",
						newRate);
					break;
				case AccountType.Savings:
					sb.AppendFormatLine("Your savings interest rate has changed to {0}%", newRate);
					break;
				case AccountType.Credit:
					sb.AppendFormatLine(
						"The interest rate for which you will be charged for new purchases is now {0}%", newRate);
					break;
			}
			if (newRate > 0.1m)
				message.Important = true;


			sb.AppendLine();
			sb.AppendLine();

			sb.AppendLine("Kind regards - your bank.");
		    return message;
		}
	}
}
