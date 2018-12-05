using System;
using FakeItEasy;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using ProviderProcessing.DoNotOpen.Infrastructure;
using ProviderProcessing.ProcessReports;
using ProviderProcessing.ProviderDatas;

namespace ProviderProcessing.DoNotOpen
{
    [TestFixture]
    public class ProviderProcessor_ManualSpecification : ReportingTest<ProviderProcessor_ManualSpecification>
    {
        [Test]
        public void FailsWithMessageAboutError_WhenWarningsAndErrors()
        {
            var providerRepository = A.Fake<ProviderRepository>();
            var productValidator = A.Fake<ProductValidator>();
            var processor = A.Fake<ProviderProcessor>(
                opt =>
                    opt.WithArgumentsForConstructor(
                        new object[] { providerRepository, productValidator }));

            //Пусть existingData не задана.
            A.CallTo(() => providerRepository.FindByProviderId(A.Dummy<Guid>()))
                .WithAnyArguments().Returns(null);

            //Если в данных содержится 2 продукта,
            var data = new ProviderData
            {
                ProviderId = Guid.NewGuid(),
                Products = new[]
                {
                    new ProductData {Id = Guid.NewGuid()},
                    new ProductData {Id = Guid.NewGuid()}
                }
            };
            var message = JsonConvert.SerializeObject(data);

            //причем у одного из них неверная цена,
            var product0 = data.Products[0];
            var validationResult0 = new ProductValidationResult(
                product0, "Bad price", ProductValidationSeverity.Warning);
            A.CallTo(() => productValidator.ValidateProduct(product0))
                .Returns(new[] {validationResult0});
            
            //а у другого продукта неизвестное имя,
            var product1 = data.Products[1];
            var validationResult1 = new ProductValidationResult(
                product1, "Unknown product name", ProductValidationSeverity.Error);
            A.CallTo(() => productValidator.ValidateProduct(product1))
                .Returns(new[] { validationResult1 });

            //то протокол операции должен содержать сообщение о неизвестном имени продукта.
            processor.ProcessProviderData(message).ToString()
                .Should().Contain("Unknown product name");
        }
    }
}