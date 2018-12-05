namespace Emails
{
    public class Config
    {
        public static Config Local = new Config {IncreaseRate = true, IncreaseRateFactor = 1.01m};

        public bool IncreaseRate { get; set; }
        public decimal IncreaseRateFactor { get; set; }
    }
}