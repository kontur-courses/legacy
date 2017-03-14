using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FakeItEasy;
using log4net.Appender;
using log4net.Config;
using NUnit.Framework;

namespace ProviderProcessing.Solved_WithCharacterizationTests
{
	[TestFixture]
	[UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
	public class ProviderProcessor_Should
	{
		private IProductsReference productsReference;
		private IUnitsReference unitsReference;
		private Settings settings;
		private IProviderRepository providerRepo;
		private ProviderProcessor processor;
		private MemoryAppender memoryAppender;

		[SetUp]
		public void SetUp()
		{
			productsReference = A.Fake<IProductsReference>();
			A.CallTo((Expression<Action>) (() => productsReference.FindCodeByName("unknown"))).Returns(null);
			A.CallTo((Expression<Action>) (() => productsReference.FindCodeByName("known"))).Returns(42);

			unitsReference = A.Fake<IUnitsReference>();
			A.CallTo(() => unitsReference.FindByCode("unknown")).Returns(null);
			A.CallTo(() => unitsReference.FindByCode("known")).Returns(new MeasureUnit());

			settings = new Settings() { MaxPossiblePrice = 100500 };
			providerRepo = A.Fake<IProviderRepository>();

			processor = new ProviderProcessor(providerRepo, productsReference, unitsReference, settings);
			memoryAppender = new MemoryAppender();
			BasicConfigurator.Configure(memoryAppender);
		}

		private static Func<ProviderData> emptyProviderData = () => new ProviderData
		{
			Id = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1),
			ProviderId = new Guid(2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2),
			ReplaceData = false,
			Timestamp = new DateTime(2017, 01, 02),
			Products = new ProductData[0]
		};

		private static IEnumerable<TestCaseData> GetTestCases()
		{
			yield return new TestCaseData(emptyProviderData(), null) { TestName = "New data" };
			var older = emptyProviderData();
			older.Timestamp = older.Timestamp - new TimeSpan(10000);
			yield return new TestCaseData(older, emptyProviderData()) { TestName = "Old" };
			yield return new TestCaseData(emptyProviderData(), emptyProviderData()) { TestName = "Same empty data" };
			var withReplace = emptyProviderData();
			withReplace.ReplaceData = true;
			yield return new TestCaseData(withReplace, emptyProviderData()) { TestName = "Same empty data with replace" };
			var withUnknownName = emptyProviderData();
			withUnknownName.Products = new[] { new ProductData() { Name = "unknown" } };
			yield return new TestCaseData(withUnknownName, null) { TestName = "Unknown product name" };
			var withBadPrice = emptyProviderData();
			withBadPrice.Products = new[] { new ProductData() { Name = "known", Price = -1, UnitsCode = "known"} };
			yield return new TestCaseData(withBadPrice, null) { TestName = "Product with bad price" };
			var withBadUnits = emptyProviderData();
			withBadUnits.Products = new[] { new ProductData() { Name = "known", Price = 1, UnitsCode = "unknown" } };
			yield return new TestCaseData(withBadUnits, null) { TestName = "Product with bad unit" };
			var data = emptyProviderData();
			data.Products = new[]
			{
				new ProductData() { Name = "known", Price = -1, UnitsCode = "known" },
				new ProductData() { Name = "unknown", Price = 1, UnitsCode = "known" }
			};
			yield return new TestCaseData(data, null) { TestName = "Two errors" };
		}

		[TestCaseSource("GetTestCases")]
		[Test]
		public void CharacterizationTest(ProviderData receivedData, ProviderData existedData)
		{
			using (ApprovalResults.ForScenario(TestContext.CurrentContext.Test.Name))
			{
				var res = processor.ProcessProviderData(receivedData, existedData);
				var s = res + "\n" + string.Join("\n", memoryAppender.GetEvents().Select(e => e.RenderedMessage));
				Approvals.Verify(s);
			}
		}
	}
}