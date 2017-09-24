using System;
using ProviderProcessing.MeasureUnits;

namespace ProviderProcessing.Solved_WithCharacterizationTests
{
	public interface IUnitsReference
	{
		MeasureUnit FindByCode(string unitsCode);
	}

	public class UnitsReference : IUnitsReference
	{
		private static UnitsReference instance;
		public static UnitsReference GetInstance()
		{
			return instance ?? (instance = LoadReference());
		}

		public static UnitsReference LoadReference()
		{
			throw new NotImplementedException("Долгая-долгая инициализация справочника.");
		}

		public MeasureUnit FindByCode(string unitsCode)
		{
			throw new NotImplementedException("Работа со справочником");
		}

		// Прочие методы
	}
}