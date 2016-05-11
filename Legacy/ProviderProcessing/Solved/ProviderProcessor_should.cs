using System;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Legacy.ProviderProcessing.Solved
{
	[TestFixture]
	public class ProviderProcessor_Should
	{
		private ProviderProcessor processor;
	    private ProviderRepository providerRepository;
	    private ProductValidator productValidator;
	    private ProviderData newestData;
	    private ProviderData oldData;

	    [SetUp]
		public void SetUp()
	    {
	        var providerId = Guid.NewGuid();
            var product = new ProductData
            {
                Name = "abc"
            };
            
            newestData = new ProviderData
            {
                ProviderId = providerId,
                Timestamp = new DateTime(2016, 7, 1),
                Products = new []{product},
            };

	        oldData = new ProviderData
	        {
	            ProviderId = providerId,
	            Timestamp = new DateTime(2016, 6, 1),
	            Products = new ProductData[0]
	        };

            providerRepository = A.Fake<ProviderRepository>();
	        productValidator = A.Fake<ProductValidator>();
            processor = new ProviderProcessor(providerRepository, productValidator);
        }

        [Test]
        [UseReporter(typeof(DiffReporter))]
        public void Ignore_OutdatedData()
	    {
	        A.CallTo(() => providerRepository.FindByProviderId(oldData.ProviderId))
                .Returns(newestData);

	        var report = processor.ProcessProviderData(oldData);
            Approvals.Verify(report);
		}

        [Test]
        [UseReporter(typeof(DiffReporter))]
        public void FailWithDetailedInfo_OnValidationErrors()
        {
            A.CallTo(() => providerRepository.FindByProviderId(newestData.ProviderId))
                .Returns(null);
            var validationResult = new ProductValidationResult(
                newestData.Products.First(),
                "Achtung!",
                ProductValidationSeverity.Error);
            A.CallTo(() => productValidator.ValidateProduct(null))
                .WithAnyArguments()
                .Returns(validationResult);

            var report = processor.ProcessProviderData(newestData);

            Approvals.Verify(report);
            A.CallTo(() => providerRepository.Save(null))
                .WithAnyArguments()
                .MustNotHaveHappened();
        }

        [Test]
        public void SaveData_ForNewProvider()
		{
            A.CallTo(() => providerRepository.FindByProviderId(newestData.ProviderId))
                .Returns(null);
            A.CallTo(() => productValidator.ValidateProduct(null))
                .WithAnyArguments()
                .Returns(null);

            var report = processor.ProcessProviderData(newestData);

            report.Success.Should().BeTrue();
            A.CallTo(() => providerRepository.Save(newestData))
                .MustHaveHappened();
        }

        [Test]
        public void UpdateData_WhenOldExists()
        {
            var product1 = new ProductData
            {
                Id = Guid.NewGuid(),
                Name = "old1"
            };
            var product2 = new ProductData
            {
                Id = Guid.NewGuid(),
                Name = "old2"
            };
            oldData.Products = new[] { product1, product2 };

            var newProduct2 = new ProductData
            {
                Id = product2.Id,
                Name = "new2"
            };
            var newProduct3 = new ProductData
            {
                Id = Guid.NewGuid(),
                Name = "new3"
            };
            newestData.Products = new[] { newProduct2, newProduct3 };

            A.CallTo(() => providerRepository.FindByProviderId(newestData.ProviderId))
                .Returns(oldData);
            A.CallTo(() => productValidator.ValidateProduct(null))
                .WithAnyArguments()
                .Returns(null);

            var report = processor.ProcessProviderData(newestData);

            report.Success.Should().BeTrue();
            A.CallTo(() => providerRepository.Update(oldData))
                .MustHaveHappened();
            A.CallTo(() => providerRepository.Save(null))
                .WithAnyArguments()
                .MustNotHaveHappened();
        }

        [Test]
        public void ReplaceData_WhenAsked()
        {
            newestData.ReplaceData = true;

            A.CallTo(() => providerRepository.FindByProviderId(newestData.ProviderId))
                .Returns(oldData);
            A.CallTo(() => productValidator.ValidateProduct(null))
                .WithAnyArguments()
                .Returns(null);

            var report = processor.ProcessProviderData(newestData);

            report.Success.Should().BeTrue();
            A.CallTo(() => providerRepository.Save(newestData))
                .MustHaveHappened();
            A.CallTo(() => providerRepository.RemoveById(oldData.Id))
                .MustHaveHappened();
        }
        
        [Test]
	    public void FailsWithMessageAboutError_WhenWarningsAndErrors()
	    {
            //Если в данных содержится 2 продукта,
	        var data = new ProviderData
            {
                ProviderId = Guid.NewGuid(),
                Products = new[]
                {
                    new ProductData { Id = Guid.NewGuid() },
                    new ProductData { Id = Guid.NewGuid() }
                }
            };
            //причем у одного из них неверная цена,
            var validationResult0 = new ProductValidationResult(
                data.Products[0], "Bad price", ProductValidationSeverity.Warning);
            //а у другого продукта неизвестное имя,
            var validationResult1 = new ProductValidationResult(
                data.Products[1], "Unknown product name", ProductValidationSeverity.Error);

            A.CallTo(() => productValidator.ValidateProduct(data.Products[0]))
                .Returns(validationResult0).Once();
            A.CallTo(() => productValidator.ValidateProduct(data.Products[1]))
                .Returns(validationResult1).Once();

            //то протокол операции должен содержать сообщение о неизвестном имени продукта.
            processor.ProcessProviderData(data).ToString()
                .Should().Contain("Unknown product name");
        }
    }
}