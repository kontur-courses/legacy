using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FakeItEasy;
using log4net.Appender;
using log4net.Config;
using NUnit.Framework;
using ProviderProcessing.Solved.ProviderDatas;
using ProviderProcessing.Solved.References;

namespace ProviderProcessing.Solved
{
	[TestFixture]
	[UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
	public class ProviderProcessor_Should
	{
		private ProductsReference productsReference;
		private MeasureUnitsReference measureUnitsReference;
		private ProviderProcessor processor;
		private MemoryAppender memoryAppender;

		[SetUp]
		public void SetUp()
		{
			productsReference = A.Fake<ProductsReference>();
			A.CallTo(() => productsReference.FindCodeByName("unknown")).Returns(null);
			A.CallTo(() => productsReference.FindCodeByName("known")).Returns(42);
			ProductsReference.SetInstance(productsReference);

			measureUnitsReference = A.Fake<MeasureUnitsReference>();
			A.CallTo(() => measureUnitsReference.FindByCode("unknown")).Returns(null);
			A.CallTo(() => measureUnitsReference.FindByCode("known")).Returns(new MeasureUnit());
			MeasureUnitsReference.SetInstance(measureUnitsReference);

			processor = new ProviderProcessor(A.Fake<ProviderRepository>(), null);
			memoryAppender = new MemoryAppender();
			BasicConfigurator.Configure(memoryAppender);
		}

		private static readonly Func<ProviderData> emptyProviderData = () => new ProviderData
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
			withBadPrice.Products = new[] { new ProductData() { Name = "known", Price = -1, MeasureUnitCode = "known" } };
			yield return new TestCaseData(withBadPrice, null) { TestName = "Product with bad price" };

			var withBadUnits = emptyProviderData();
			withBadUnits.Products = new[] { new ProductData() { Name = "known", Price = 1, MeasureUnitCode = "unknown" } };
			yield return new TestCaseData(withBadUnits, null) { TestName = "Product with bad unit" };

			var data = emptyProviderData();
			data.Products = new[]
			{
				new ProductData { Name = "known", Price = -1, MeasureUnitCode = "known" },
				new ProductData { Name = "unknown", Price = 1, MeasureUnitCode = "known" }
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