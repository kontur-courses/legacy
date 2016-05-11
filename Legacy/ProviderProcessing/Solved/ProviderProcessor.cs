using System.Linq;
using log4net;
using Newtonsoft.Json;

namespace Legacy.ProviderProcessing.Solved
{
    public class ProviderProcessor
	{
		private readonly ProviderRepository repository;
	    private readonly ProductValidator validator;

        public ProviderProcessor(ProviderRepository repository, ProductValidator validator)
        {
            this.repository = repository;
            this.validator = validator;
        }

	    public ProcessReport ProcessProviderData(string message)
		{
			var data = JsonConvert.DeserializeObject<ProviderData>(message);
			return ProcessProviderData(data);
		}

		public ProcessReport ProcessProviderData(ProviderData data)
		{
			var existingData = repository.FindByProviderId(data.ProviderId);
			if (existingData != null && data.Timestamp < existingData.Timestamp)
			{
			    LogOutdated(data, existingData);
			    return new ProcessReport(false, "Outdated data");
			}

            var errors = data.Products
		        .Select(p => validator.ValidateProduct(p))
                .Where(p => p != null)
                .OrderBy(p => p.Severity)
		        .ToArray();
		    if (errors.Any())
		        return new ProcessReport(false, "Product validation errors", errors);

		    SaveDataOrUpdateExisting(data, existingData);
		    return new ProcessReport(true, "OK");
		}

	    private void SaveDataOrUpdateExisting(ProviderData data, ProviderData existingData)
	    {
	        if (existingData == null)
	        {
	            repository.Save(data);
                return;
	        }

	        if (data.ReplaceData)
	        {
	            LogReplace(data, existingData);
	            repository.RemoveById(existingData.Id);
	            repository.Save(data);
	            return;
	        }

	        var actualProducts = existingData.Products
	            .Where(p => data.Products.All(d => d.Id != p.Id))
	            .ToList();
	        var updatedCount = existingData.Products.Length - actualProducts.Count;
	        var newCount = data.Products.Length - updatedCount;
	        LogUpdate(data, newCount, updatedCount);

	        existingData.Products = actualProducts
	            .Concat(data.Products)
	            .ToArray();
	        repository.Update(existingData);
	        LogOk(data, existingData);
	    }

        private static void LogOutdated(ProviderData data, ProviderData existingData)
	    {
	        log.InfoFormat("Outdated provider data. ProviderId {0} Received timestamp: {1} database timestamp {2}",
	            data.ProviderId, data.Timestamp, existingData.Timestamp);
	    }

	    private static void LogReplace(ProviderData data, ProviderData existingData)
	    {
	        log.InfoFormat("Provider {0} products replaced. Deleted: {1} Added: {2}",
	            data.ProviderId, existingData.Products.Length, data.Products.Length);
	    }

	    private static void LogUpdate(ProviderData data, int newCount, int updatedCount)
	    {
	        log.InfoFormat("Provider {0} products update. New: {1} Updated: {2}",
	            data.ProviderId, newCount, updatedCount);
	    }

	    private void LogOk(ProviderData data, ProviderData existingData)
	    {
	        log.InfoFormat("New data {0}, Existing data {1}", FormatData(data), FormatData(existingData));
	    }

	    private string FormatData(ProviderData data)
		{
			return data.Id + " for " + data.ProviderId + " products count " + data.Products.Length;
		}

		private static readonly ILog log = LogManager.GetLogger(typeof(ProviderProcessor));
	}
}