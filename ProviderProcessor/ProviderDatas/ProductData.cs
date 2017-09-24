using System;

namespace ProviderProcessing.ProviderDatas
{
	public class ProductData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public string MeasureUnitCode { get; set; }
	}
}