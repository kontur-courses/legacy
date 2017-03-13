using System.Collections.Generic;
using System.Linq;
using log4net;
using Newtonsoft.Json;

namespace ProviderProcessing
{
	public class ProviderProcessor
	{
		private readonly ProviderRepository repo;

		public ProviderProcessor()
		{
			repo = new ProviderRepository();
		}

		public ProcessReport ProcessProviderData(string message)
		{
			var data = JsonConvert.DeserializeObject<ProviderData>(message);
			var existingData = repo.FindByProviderId(data.ProviderId);
			if (existingData != null && data.Timestamp < existingData.Timestamp)
			{
				log.InfoFormat("Outdated provider data. ProviderId {0} Received timestamp: {1} database timestamp {2}",
					data.ProviderId, data.Timestamp, existingData.Timestamp);
				return new ProcessReport(false, "Outdated data");
			}
			var errors = ValidateNames(data.Products)
				.Concat(ValidatePricesAndUnitsCodes(data.Products))
				.ToList();
			if (errors.Any())
			{
				return new ProcessReport(false, "Product validation errors",
					errors.ToArray());
			}

			if (existingData == null)
			{
				repo.Save(data);
			}
			else if (data.ReplaceData)
			{
				log.InfoFormat("Provider {0} products replaced. Deleted: {1} Added: {2}",
					data.ProviderId, existingData.Products.Length, data.Products.Length);
				repo.RemoveById(existingData.Id);
				repo.Save(data);
			}
			else
			{
				var actualProducts = existingData.Products.Where(p => data.Products.All(d => d.Id != p.Id)).ToList();
				var updatedCount = existingData.Products.Length - actualProducts.Count;
				var newCount = data.Products.Length - updatedCount;
				log.InfoFormat("Provider {0} products update. New: {1} Updated: {2}",
					data.ProviderId, newCount, updatedCount);
				existingData.Products = actualProducts.Concat(data.Products).ToArray();
				repo.Update(existingData);
			}
			log.InfoFormat("New data {0}, Existing data {1}", FormatData(data), FormatData(existingData));
			return new ProcessReport(true, "OK");
		}

		private string FormatData(ProviderData data)
		{
			return data.Id + " for " + data.ProviderId + " products count " + data.Products.Length;
		}

		private IEnumerable<ProductValidationResult> ValidateNames(ProductData[] data)
		{
			var reference = ProductsReference.GetInstance();
			foreach (var product in data)
			{
				if (!reference.FindCodeByName(product.Name).HasValue)
					yield return new ProductValidationResult(product,
						"Unknown product name", ProductValidationSeverity.Error);
			}
		}

		private IEnumerable<ProductValidationResult> ValidatePricesAndUnitsCodes(ProductData[] data)
		{
			foreach (var product in data)
			{
				if (product.Price <= 0 || product.Price > Settings.Global.MaxPossiblePrice)
					yield return new ProductValidationResult(product, "Bad price", ProductValidationSeverity.Warning);
				if (!IsValidUnitsCode(product.UnitsCode))
					yield return new ProductValidationResult(product,
						"Bad units of measurements", ProductValidationSeverity.Warning);
			}
		}

		private bool IsValidUnitsCode(string unitsCode)
		{
			var reference = UnitsReference.GetInstance();
			return reference.FindByCode(unitsCode) != null;
		}

		private static readonly ILog log = LogManager.GetLogger(typeof(ProviderProcessor));
	}
}