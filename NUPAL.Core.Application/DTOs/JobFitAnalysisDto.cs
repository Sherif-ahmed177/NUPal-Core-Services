using System.Text.Json.Serialization;

namespace NUPAL.Core.Application.DTOs
{
    public class JobFitAnalysisDto
    {
        [JsonPropertyName("overallScore")]    public int OverallScore { get; set; }
        [JsonPropertyName("matchStatus")]     public string MatchStatus { get; set; } = string.Empty;
        [JsonPropertyName("detailedSummary")] public string DetailedSummary { get; set; } = string.Empty;
        
        [JsonPropertyName("highlights")]      public List<string> Highlights { get; set; } = [];
        [JsonPropertyName("opportunities")]   public List<string> Opportunities { get; set; } = [];
        
        [JsonPropertyName("recommendations")] public List<string> Recommendations { get; set; } = [];
        
        [JsonPropertyName("breakdown")]       public JobFitBreakdown Breakdown { get; set; } = new();
        
        [JsonPropertyName("actionPlan")]      public List<JobFitActionStep> ActionPlan { get; set; } = [];
        
        [JsonPropertyName("interviewFocus")]  public List<string> InterviewFocus { get; set; } = [];
        [JsonPropertyName("suggestedLearning")] public List<string> SuggestedLearning { get; set; } = [];

        [JsonPropertyName("redFlags")]        public List<string> RedFlags { get; set; } = [];

        [JsonPropertyName("jobTitle")]        public string? JobTitle { get; set; }
        [JsonPropertyName("companyName")]     public string? CompanyName { get; set; }
    }

    public class JobFitBreakdown
    {
        [JsonPropertyName("skills")]     public int Skills { get; set; }
        [JsonPropertyName("experience")] public int Experience { get; set; }
        [JsonPropertyName("domain")]     public int Domain { get; set; }
        [JsonPropertyName("credentials")] public int Credentials { get; set; }
        [JsonPropertyName("readiness")]  public int Readiness { get; set; }

        [JsonPropertyName("skillsNote")]   public string? SkillsNote { get; set; }
        [JsonPropertyName("experienceNote")] public string? ExperienceNote { get; set; }
        [JsonPropertyName("domainNote")]   public string? DomainNote { get; set; }
        [JsonPropertyName("credentialsNote")] public string? CredentialsNote { get; set; }

        [JsonPropertyName("matchedSkills")] public List<MatchedSkill> MatchedSkills { get; set; } = [];
        [JsonPropertyName("missingSkills")] public List<MissingSkill> MissingSkills { get; set; } = [];
    }

    public class MatchedSkill
    {
        [JsonPropertyName("skill")] public string Skill { get; set; } = string.Empty;
        [JsonPropertyName("evidence")] public string Evidence { get; set; } = string.Empty;
        [JsonPropertyName("level")] public string Level { get; set; } = string.Empty;
    }

    public class MissingSkill
    {
        [JsonPropertyName("skill")] public string Skill { get; set; } = string.Empty;
        [JsonPropertyName("importance")] public string Importance { get; set; } = string.Empty;
        [JsonPropertyName("fixable")] public string Fixable { get; set; } = string.Empty;
    }

    public class JobFitActionStep
    {
        [JsonPropertyName("title")]          public string Title { get; set; } = string.Empty;
        [JsonPropertyName("description")]    public string Description { get; set; } = string.Empty;
        [JsonPropertyName("targetGap")]       public string? TargetGap { get; set; }
        [JsonPropertyName("expectedImpact")] public string ExpectedImpact { get; set; } = string.Empty;
        [JsonPropertyName("priority")]       public string Priority { get; set; } = "Medium";
        [JsonPropertyName("status")]         public string Status { get; set; } = "Do soon"; // "Do now", "Do soon", "Later"
    }

    public class AnalyzeFitRequest
    {
        public string? JobUrl { get; set; }
        public string? JobDescription { get; set; }
        public string? ResumeId { get; set; }
    }
}
