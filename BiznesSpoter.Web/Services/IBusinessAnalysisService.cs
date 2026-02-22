namespace BiznesSpoter.Web.Services;

public interface IBusinessAnalysisService
{
    Task<BusinessAnalysisResult?> AnalyzeBusinessPotentialAsync(string location, string industry, double radiusMeters);
}