using System;
using System.Linq.Expressions;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using ProviderProcessing.MeasureUnits;

namespace ProviderProcessing.Solved_WithCharacterizationTests
{
	public class ProductValidator
	{
		private readonly Settings settings;
		private readonly IProductsReference productsReference;
		private readonly IUnitsReference unitsReference;

		public ProductValidator(Settings settings, IProductsReference productsReference, IUnitsReference unitsReference)
		{
			this.settings = settings;
			this.productsReference = productsReference;
			this.unitsReference = unitsReference;
		}

		public ProductValidationResult ValidateProduct(ProductData product)
		{
			if (!productsReference.FindCodeByName(product.Name).HasValue)
				return new ProductValidationResult(product,
					"Unknown product name", ProductValidationSeverity.Error);
			if (product.Price <= 0 || product.Price > settings.MaxPossiblePrice)
				return new ProductValidationResult(product, "Bad price", ProductValidationSeverity.Warning);
			if (!IsValidUnitsCode(product.UnitsCode))
				return new ProductValidationResult(product,
					"Bad units of measurements", ProductValidationSeverity.Warning);

			return new ProductValidationResult(product, "OK", ProductValidationSeverity.Ok);
		}

		private bool IsValidUnitsCode(string unitsCode)
		{
			return unitsReference.FindByCode(unitsCode) != null;
		}
	}

	[TestFixture]
	public class ProductValidator_Should
	{
		private IProductsReference productsReference;
		private IUnitsReference unitsReference;
		private ProductValidator validator;
		private Settings settings;

		[SetUp]
		public void SetUp()
		{
			productsReference = A.Fake<IProductsReference>();
			A.CallTo(() => productsReference.FindCodeByName("unknown")).Returns(null);
			A.CallTo(() => productsReference.FindCodeByName("known")).Returns(42);

			unitsReference = A.Fake<IUnitsReference>();
			A.CallTo(() => unitsReference.FindByCode("unknown")).Returns(null);
			A.CallTo(() => unitsReference.FindByCode("known")).Returns(new MeasureUnit());

			settings = new Settings() { MaxPossiblePrice = 100500 };
			validator = new ProductValidator(settings, productsReference, unitsReference);
		}

		[Test]
		public void ErrorOnUnknownName()
		{
			var product = new ProductData { Name = "unknown" };
			var result = validator.ValidateProduct(product);
			var expectedResult = new ProductValidationResult(product, "Unknown product name", ProductValidationSeverity.Error);
			result.ShouldBeEquivalentTo(expectedResult);
		}

		[TestCase(-1)]
		[TestCase(100501)]
		public void WarningOnBadPrice(decimal price)
		{
			settings.MaxPossiblePrice = 100500;
			var product = new ProductData { Name = "known", Price = price };

			var result = validator.ValidateProduct(product);

			var expectedResult = new ProductValidationResult(product, "Bad price", ProductValidationSeverity.Warning);
			result.ShouldBeEquivalentTo(expectedResult);
		}

		[Test]
		public void WarningOnUnknownUnit()
		{
			var product = new ProductData { Name = "known", UnitsCode = "unknown", Price = 1 };

			var result = validator.ValidateProduct(product);

			var expectedResult = new ProductValidationResult(product, "Bad units of measurements", ProductValidationSeverity.Warning);
			result.ShouldBeEquivalentTo(expectedResult);
		}

		[Test]
		public void Ok_WhenNoErrors()
		{
			var product = new ProductData { Name = "known", UnitsCode = "known", Price = 1 };

			var result = validator.ValidateProduct(product);

			var expectedResult = new ProductValidationResult(product, "OK", ProductValidationSeverity.Ok);
			result.ShouldBeEquivalentTo(expectedResult);
		}


	}
}