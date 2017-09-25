using System;

namespace ProviderProcessing.Solved.References
{
	public class ProductsReference
	{
		private static ProductsReference instance;

		public static ProductsReference GetInstance()
		{
			return instance ?? (instance = LoadReference());
		}

		public static void SetInstance(ProductsReference value)
		{
			instance = value;
		}

		public static ProductsReference LoadReference()
		{
			throw new NotImplementedException("Долгая-долгая инициализация справочника.");
		}

		public virtual int? FindCodeByName(string name)
		{
			throw new NotImplementedException("Работа со справочником");
		}

		// Прочие методы
	}
}