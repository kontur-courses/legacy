using ApprovalTests.Combinations;
using ApprovalTests.Reporters;
using NUnit.Framework;

namespace Emails.Solved
{
    [TestFixture]
    public class NewRateEmailMessager_Should
    {
        public delegate EmailMessage CreateMessage(string customerName, AccountType accountType, decimal rate);

        private CreateMessage GetCreateMessage(bool useRefactored)
        {
            CreateMessage legacyCreateMessage = new NewRateEmailMessager().CreateMessage;
            CreateMessage refactoredCreateMessage = new NewRateEmailMessager_Refactored().CreateMessage;
            return useRefactored ? legacyCreateMessage : refactoredCreateMessage;
        }

        [Test, TestCase(false), TestCase(true)]
        [UseReporter(typeof(DiffReporter))]
        public void CreateAllMessages(bool useRefactored)
        {
            var createMessage = GetCreateMessage(useRefactored);

            CombinationApprovals.VerifyAllCombinations(
                (customerName, accountType, rate) =>
                    createMessage(customerName, accountType, rate),
                ApprovalTestsExtensions.Print,
                new[] {"Jack", "Lily"},
                new[] {AccountType.Cheque, AccountType.Credit, AccountType.Savings},
                new[] {0.05m, 0.1m});
        }

        [Test, TestCase(false), TestCase(true)]
        [UseReporter(typeof(DiffReporter))]
        public void NotIncreaseRate_WhenItIsOffInConfig(bool useRefactored)
        {
            var createMessage = GetCreateMessage(useRefactored);

            new Config {IncreaseRate = false}
                .ApplyTo(() => createMessage("Jack", AccountType.Savings, 0.099m))
                .Print().Verify();
        }

        [Test, TestCase(false), TestCase(true)]
        [UseReporter(typeof(DiffReporter))]
        public void UseRateFromConfig(bool useRefactored)
        {
            var createMessage = GetCreateMessage(useRefactored);

            new Config {IncreaseRate = true, IncreaseRateFactor = 2.0m}
                .ApplyTo(() => createMessage("Jack", AccountType.Savings, 1.0m))
                .Print().Verify();
        }
    }
}