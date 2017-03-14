using System.Collections.Generic;
using System.Linq;
using log4net;
using Newtonsoft.Json;

namespace ProviderProcessing.Solved2
{
	public class ProviderProcessor
	{
		private readonly Settings settings;

		private readonly IProductsReference productsReference;
		private readonly ProductValidator validator;

		private readonly IUnitsReference unitsReference;

		private readonly IProviderRepository repo;

		public ProviderProcessor(IProviderRepository providerRepository, IProductsReference productsReference, IUnitsReference unitsReference, Settings settings)
		{
			this.productsReference = productsReference;
			this.unitsReference = unitsReference;
			this.settings = settings;
			validator = new ProductValidator(settings, productsReference, unitsReference);
			repo = providerRepository;
		}

		public ProviderProcessor() : this(new ProviderRepository(), ProductsReference.GetInstance(), UnitsReference.GetInstance(), Settings.Global)
		{
		}

		public ProcessReport ProcessProviderData(string message)
		{
			var data = JsonConvert.DeserializeObject<ProviderData>(message);
			ProviderData existingData = repo.FindByProviderId(data.ProviderId);
			return ProcessProviderData(data, existingData);
		}

		public ProcessReport ProcessProviderData(ProviderData receivedData, ProviderData existingData)
		{
			if (existingData != null && receivedData.Timestamp < existingData.Timestamp)
			{
				log.InfoFormat("Outdated provider data. ProviderId {0} Received timestamp: {1} database timestamp {2}",
					receivedData.ProviderId, receivedData.Timestamp, existingData.Timestamp);
				return new ProcessReport(false, "Outdated data");
			}
			var errors = receivedData.Products.Select(validator.ValidateProduct).ToList();
			if (errors.Any())
			{
				return new ProcessReport(false, "Product validation errors",
					errors.ToArray());
			}

			if (existingData == null)
			{
				repo.Save(receivedData);
			}
			else if (receivedData.ReplaceData)
			{
				log.InfoFormat("Provider {0} products replaced. Deleted: {1} Added: {2}",
					receivedData.ProviderId, existingData.Products.Length, receivedData.Products.Length);
				repo.RemoveById(existingData.Id);
				repo.Save(receivedData);
			}
			else
			{
				var actualProducts = existingData.Products.Where(p => receivedData.Products.All(d => d.Id != p.Id)).ToList();
				var updatedCount = existingData.Products.Length - actualProducts.Count;
				var newCount = receivedData.Products.Length - updatedCount;
				log.InfoFormat("Provider {0} products update. New: {1} Updated: {2}",
					receivedData.ProviderId, newCount, updatedCount);
				existingData.Products = actualProducts.Concat(receivedData.Products).ToArray();
				repo.Update(existingData);
			}
			log.InfoFormat("New data {0}, Existing data {1}", FormatData(receivedData), FormatData(existingData));
			return new ProcessReport(true, "OK");
		}

		private string FormatData(ProviderData data)
		{
			if (data == null) return "null";
			return data.Id + " for " + data.ProviderId + " products count " + data.Products.Length;
		}

		private IEnumerable<ProductValidationResult> ValidateNames(ProductData[] data)
		{
			foreach (var product in data)
			{
				if (!productsReference.FindCodeByName(product.Name).HasValue)
					yield return new ProductValidationResult(product,
						"Unknown product name", ProductValidationSeverity.Error);
			}
		}

		private IEnumerable<ProductValidationResult> ValidatePricesAndUnitsCodes(ProductData[] data)
		{
			foreach (var product in data)
			{
				if (product.Price <= 0 || product.Price > settings.MaxPossiblePrice)
					yield return new ProductValidationResult(product, "Bad price", ProductValidationSeverity.Warning);
				if (!IsValidUnitsCode(product.UnitsCode))
					yield return new ProductValidationResult(product,
						"Bad units of measurements", ProductValidationSeverity.Warning);
			}
		}

		private bool IsValidUnitsCode(string unitsCode)
		{
			return unitsReference.FindByCode(unitsCode) != null;
		}

		private static readonly ILog log = LogManager.GetLogger(typeof(ProviderProcessor));
	}
}