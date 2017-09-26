using System.Text;

namespace Emails.Solved
{
	public class NewRateEmailMessager_Refactored
	{
		public EmailMessage CreateMessage(string customerName, AccountType accountType, decimal rate)
		{
			var newRate = IncreaseRate(rate);
			var isImportant = IsImportant(newRate);
			return CreateMessage(customerName, accountType,
				newRate, isImportant);
		}

		private static decimal IncreaseRate(decimal rate)
		{
			return Config.Local.IncreaseRate ? rate * Config.Local.IncreaseRateFactor : rate;
		}

		private static bool IsImportant(decimal rate)
		{
			return rate > 0.1m;
		}

		private static EmailMessage CreateMessage(string customerName, AccountType accountType,
			decimal rate, bool isImportant)
		{
			return new EmailMessage
			{
				Subject = "New rate!",
				Important = isImportant,
				Body = BuildBody(customerName, accountType, rate)
			};
		}

		private static StringBuilder BuildBody(
			string customerName, AccountType accountType, decimal newRate)
		{
			var sb = new StringBuilder();

			sb.AppendFormatLine("Dear {0}", customerName);
			sb.AppendLine();

			sb.Append("We are sending you this message with respect to your ");
			sb.Append(GetAccountName(accountType));
			sb.AppendLine();
			sb.AppendLine();

			sb.AppendLine(GetNewInterestAccountType(accountType, newRate));
			sb.AppendLine();
			sb.AppendLine();

			sb.AppendLine("Kind regards - your bank.");

			return sb;
		}

		private static string GetAccountName(AccountType accountType)
		{
			switch (accountType)
			{
				case AccountType.Cheque:
					return "chequing account";
				case AccountType.Savings:
					return "on line savings account";
				case AccountType.Credit:
					return "credit card";
			}
			return "unknown";
		}

		private static string GetNewInterestAccountType(AccountType accountType, decimal rate)
		{
			switch (accountType)
			{
				case AccountType.Cheque:
					return $"The interest rate at which you earn interest has changed to {rate}%.";
				case AccountType.Savings:
					return $"Your savings interest rate has changed to {rate}%";
				case AccountType.Credit:
					return $"The interest rate for which you will be charged for new purchases is now {rate}%";
			}
			return "unknown";
		}
	}
}