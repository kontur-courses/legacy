using System;

namespace ProviderProcessing.Solved.ProviderDatas
{
	public class ProductData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public string MeasureUnitCode { get; set; }

		protected bool Equals(ProductData other)
		{
			return Id.Equals(other.Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ProductData) obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}