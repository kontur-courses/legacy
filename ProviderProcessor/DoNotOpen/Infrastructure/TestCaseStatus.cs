using System;

namespace ProviderProcessing.DoNotOpen.Infrastructure
{
    public class TestCaseStatus
    {
        public string TestMethod;
        public string TestName;
        public DateTime FirstRunTime;
        public DateTime LastRunTime;
        public bool Succeeded;
    }
}