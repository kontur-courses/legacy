using System;

namespace ProviderProcessing.References
{
	public class MeasureUnitsReference
	{
		private static MeasureUnitsReference instance;

		public static MeasureUnitsReference GetInstance()
		{
			return instance ?? (instance = LoadReference());
		}

		public static MeasureUnitsReference LoadReference()
		{
			throw new NotImplementedException("Долгая-долгая инициализация справочника.");
		}

		public MeasureUnit FindByCode(string measureUnitCode)
		{
			throw new NotImplementedException("Работа со справочником");
		}

		// Прочие методы
	}
}