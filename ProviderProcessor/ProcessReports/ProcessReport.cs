using System.Linq;

namespace ProviderProcessing.ProcessReports
{
    public class ProcessReport
    {
        public readonly bool Success;
        public readonly string Error;
        public readonly ProductValidationResult[] ProductValidationResults;

        public ProcessReport(bool success, string error,
            params ProductValidationResult[] productValidationResults)
        {
            Success = success;
            Error = error;
            ProductValidationResults = productValidationResults;
        }

        protected bool Equals(ProcessReport other)
        {
            return Success == other.Success &&
                   string.Equals(Error, other.Error) &&
                   ProductValidationResults.SequenceEqual(other.ProductValidationResults);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProcessReport) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Success.GetHashCode();
                hashCode = (hashCode * 397) ^ (Error != null ? Error.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"Success: {Success}, Error: {Error}, Most Critical Message:\r\n{GetMostCriticalMessage()?.Message}";
        }

        private ProductValidationResult GetMostCriticalMessage()
        {
            return ProductValidationResults.FirstOrDefault();
        }
    }
}