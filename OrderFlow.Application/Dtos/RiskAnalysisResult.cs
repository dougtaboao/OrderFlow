namespace OrderFlow.Application.Dtos
{
    public class RiskAnalysisResult
    {
        public bool Approved { get; set; }

        public string Reason { get; set; } = string.Empty;

        public static RiskAnalysisResult Approve()
        {
            return new RiskAnalysisResult
            {
                Approved = true,
                Reason = "Risk analysis approved."
            };
        }

        public static RiskAnalysisResult Reject(string reason)
        {
            return new RiskAnalysisResult
            {
                Approved = false,
                Reason = reason
            };
        }
    }
}