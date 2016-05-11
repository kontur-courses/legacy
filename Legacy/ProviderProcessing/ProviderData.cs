using System;

namespace Legacy.ProviderProcessing
{
	public class ProviderData
	{
		public Guid ProviderId;
		public DateTime Timestamp;
		public bool ReplaceData { get; set; }
		public Guid Id { get; set; }
		public ProductData[] Products { get; set; }
	}
}