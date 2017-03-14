using System;

namespace ProviderProcessing.Solved_WithCharacterizationTests
{
	public interface IProviderRepository
	{
		ProviderData FindByProviderId(Guid providerId);
		void RemoveById(Guid id);
		void Save(ProviderData data);
		void Update(ProviderData existingData);
	}

	public class ProviderRepository : IProviderRepository
	{
		public ProviderData FindByProviderId(Guid providerId)
		{
			throw new NotImplementedException("Работа с базой данных");
		}

		public void RemoveById(Guid id)
		{
			throw new NotImplementedException("Работа с базой данных");
		}

		public void Save(ProviderData data)
		{
			throw new NotImplementedException("Работа с базой данных");
		}

		public void Update(ProviderData existingData)
		{
			throw new NotImplementedException("Работа с базой данных");
		}
	}
}