using System;

namespace ProviderProcessing.ProviderDatas
{
	public class ProviderData
	{
		public Guid Id { get; set; }
		public Guid ProviderId;
		public DateTime Timestamp;
		public bool ReplaceData { get; set; }
		public ProductData[] Products { get; set; }

		protected bool Equals(ProviderData other)
		{
			return Id.Equals(other.Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ProviderData) obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}