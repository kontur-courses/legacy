using System.Diagnostics;
using System.IO;
using ApprovalTests.Core;

namespace Samples.Reporters
{
	/*
	При написании характеризационных тестов на работающий код
	может возникнуть желание заапрувить множество тестов сразу.
	Для этого можно написать специальный Reporter.

	Его нельзя использовать после, иначе ваши тесты будут вечнозелеными и бесполезными!

	Код взят тут: https://stackoverflow.com/questions/37604285/how-do-i-automatically-approve-approval-tests-when-i-run-them
	*/
	public class AutoApproveReporter : IReporterWithApprovalPower
	{
		public static readonly AutoApproveReporter INSTANCE = new AutoApproveReporter();

		private string approved;
		private string received;

		public void Report(string approved, string received)
		{
			this.approved = approved;
			this.received = received;
			Trace.WriteLine(string.Format(@"Will auto-copy ""{0}"" to ""{1}""", received, approved));
		}

		public bool ApprovedWhenReported()
		{
			if (!File.Exists(received)) return false;
			File.Delete(approved);
			if (File.Exists(approved)) return false;
			File.Copy(received, approved);
			if (!File.Exists(approved)) return false;

			return true;
		}
	}
}