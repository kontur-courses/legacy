using ProviderProcessing.Solved.ProviderDatas;

namespace ProviderProcessing.Solved.ProcessReports
{
    public class ProductValidationResult
    {
        public ProductValidationResult(ProductData product,
            string message,
            ProductValidationSeverity severity)
        {
            Product = product;
            Message = message;
            Severity = severity;
        }

        public ProductData Product { get; set; }
        public string Message { get; set; }
        public ProductValidationSeverity Severity { get; set; }
    }
}