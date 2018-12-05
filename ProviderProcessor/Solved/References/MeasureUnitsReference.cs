using System;

namespace ProviderProcessing.Solved.References
{
    public class MeasureUnitsReference
    {
        private static MeasureUnitsReference instance;

        public static MeasureUnitsReference GetInstance()
        {
            return instance ?? (instance = LoadReference());
        }

        public static void SetInstance(MeasureUnitsReference value)
        {
            instance = value;
        }

        public static MeasureUnitsReference LoadReference()
        {
            throw new NotImplementedException("Долгая-долгая инициализация справочника.");
        }

        public virtual MeasureUnit FindByCode(string measureUnitCode)
        {
            throw new NotImplementedException("Работа со справочником");
        }

        // Прочие методы
    }
}