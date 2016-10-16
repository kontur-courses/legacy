namespace ProviderProcessing.Solved
{
    public class ProductValidator
    {
        private readonly ProductsReference productsReference;
        private readonly UnitsReference unitsReference;

        public ProductValidator(ProductsReference productsReference, UnitsReference unitsReference)
        {
            this.productsReference = productsReference;
            this.unitsReference = unitsReference;
        }

        public virtual ProductValidationResult ValidateProduct(ProductData product)
        {
            if (!IsValidName(product.Name))
                return new ProductValidationResult(product,
                    "Unknown product name", ProductValidationSeverity.Error);
            if (product.Price <= 0 || product.Price > Settings.Global.MaxPossiblePrice)
                return new ProductValidationResult(product,
                    "Bad price", ProductValidationSeverity.Warning);
            if (!IsValidUnitsCode(product.UnitsCode))
                return new ProductValidationResult(product,
                    "Bad units of measurements", ProductValidationSeverity.Warning);
            return null;
        }

        private bool IsValidName(string name)
        {
            return productsReference.FindCodeByName(name).HasValue;
        }

        private bool IsValidUnitsCode(string unitsCode)
        {
            return unitsReference.FindByCode(unitsCode) != null;
        }
    }
}