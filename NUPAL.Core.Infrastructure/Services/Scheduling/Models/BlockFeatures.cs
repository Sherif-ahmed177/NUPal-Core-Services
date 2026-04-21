namespace NUPAL.Core.Infrastructure.Services.Scheduling.Models
{

    internal sealed class BlockFeatures
    {
        public string BlockId { get; set; } = "";
        public string Level   { get; set; } = "";
        public HashSet<string> Courses { get; set; } = [];
        public HashSet<string> Instructors { get; set; } = [];
        public HashSet<string> Days { get; set; } = [];
        public HashSet<int> AllSlots { get; set; } = [];
        public int NumDays { get; set; }
        public double TotalHours { get; set; }
        public Dictionary<string, List<int>> DaySlots { get; set; } = [];
    }
}
