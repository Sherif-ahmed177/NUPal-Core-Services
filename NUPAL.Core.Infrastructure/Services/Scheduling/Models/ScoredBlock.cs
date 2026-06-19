using NUPAL.Core.Application.DTOs;

namespace NUPAL.Core.Infrastructure.Services.Scheduling.Models
{
    internal sealed class ScoredBlock
    {
        public string BlockId { get; set; } = "";
        public string Level   { get; set; } = "";
        public double FinalScore  { get; set; }
        public double Similarity  { get; set; }
        public double Coverage    { get; set; }
        public double Compactness { get; set; }
        public double DayBonus    { get; set; }
        public int    NumDays    { get; set; }
        public double TotalHours { get; set; }
        public double MaxGapH    { get; set; }
        public List<string> Courses     { get; set; } = [];
        public List<string> Instructors { get; set; } = [];
        public List<string> Days        { get; set; } = [];
        public RawBlockDto Raw { get; set; } = new();
    }
}
