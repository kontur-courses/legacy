using System;
using FluentAssertions;
using NUnit.Framework;

namespace ProviderProcessing.DoNotOpen
{
    [TestFixture]
    public class ProviderProcessor_ManualSpecification
    {
        private ProviderProcessor processor;
        private ProviderRepository providerRepository;

        //[SetUp]
        //public void SetUp()
        //{
        //    providerRepository = A.Fake<ProviderRepository>();
        //    processor = A.Fake<ProviderProcessor>(
        //        opt =>
        //        opt.WithArgumentsForConstructor(
        //            new[] { providerRepository }));
        //}

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

            //то протокол операции должен содержать сообщение о неизвестном имени продукта.
            processor.ProcessProviderData(data).ToString()
                .Should().Contain("Unknown product name");
        }
    }
}
