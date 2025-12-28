using System.Text.Json.Serialization;
using Nupal.Domain.Entities;

namespace NUPAL.Core.Application.DTOs
{
    public class AgentHistoryMessageDto
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = "user"; // user/assistant

        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "unknown"; // rag/rl/unknown

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    public class AgentRouteRequestDto
    {
        [JsonPropertyName("student_id")]
        public string StudentId { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("history")]
        public List<AgentHistoryMessageDto> History { get; set; } = new();

        [JsonPropertyName("rl_recommendation")]
        public AgentRlRecommendationDto? RlRecommendation { get; set; }
    }

    public class AgentRlRecommendationDto
    {
        [JsonPropertyName("term_index")]
        public int? TermIndex { get; set; }

        [JsonPropertyName("courses")]
        public List<string> Courses { get; set; } = new();

        [JsonPropertyName("slates_by_term")]
        public List<TermRecommendation>? SlatesByTerm { get; set; }

        [JsonPropertyName("metrics")]
        public RecommendationMetrics? Metrics { get; set; }

        [JsonPropertyName("model_version")]
        public string? ModelVersion { get; set; }

        [JsonPropertyName("policy_version")]
        public string? PolicyVersion { get; set; }
    }

    public class AgentRouteResponseDto
    {
        [JsonPropertyName("intent")]
        public string Intent { get; set; } = "faq"; // faq/recommendation/mixed

        [JsonPropertyName("results")]
        public List<AgentResultDto> Results { get; set; } = new();
    }

    public class AgentResultDto
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "rag"; // rag/rl

        [JsonPropertyName("answer")]
        public string Answer { get; set; } = string.Empty;

        // arbitrary JSON object as string
        [JsonPropertyName("metadata")]
        public object? Metadata { get; set; }
    }
}
