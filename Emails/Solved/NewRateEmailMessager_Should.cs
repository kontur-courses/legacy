using ApprovalTests.Combinations;
using ApprovalTests.Reporters;
using NUnit.Framework;

namespace Emails.Solved
{
    [TestFixture]
    public class NewRateEmailMessager_Should
    {
        private NewRateEmailMessager messager;

        [SetUp]
        public void SetUp()
        {
            messager = new NewRateEmailMessager();
        }

        [Test]
        [UseReporter(typeof(DiffReporter))]
        public void CreateMessage()
        {
            CombinationApprovals.VerifyAllCombinations(
                (customerName, accountType, rate) =>
                        messager.CreateMessage(customerName, accountType, rate),
                ApprovalTestsExtensions.Print,
                new[] {"Jack", "Lily"},
                new[] {AccountType.Cheque, AccountType.Credit, AccountType.Savings},
                new[] {0.05m, 0.1m});
        }

        [Test]
        [UseReporter(typeof(DiffReporter))]
        public void NotIncreaseRate_WhenItIsOffInConfig()
        {
            new Config {IncreaseRate = false}
                .ApplyTo(() => messager.CreateMessage("Jack", AccountType.Savings, 0.099m))
                .Print().Verify();
        }

        [Test]
        [UseReporter(typeof(DiffReporter))]
        public void UseRateFromConfig()
        {
            new Config {IncreaseRate = true, IncreaseRateFactor = 2.0m}
                .ApplyTo(() => messager.CreateMessage("Jack", AccountType.Savings, 1.0m))
                .Print().Verify();
        }
    }
}