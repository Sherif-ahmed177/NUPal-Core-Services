namespace NUPAL.Core.Infrastructure.Services.Scheduling.Models
{
    internal sealed class BlockVocab
    {
        public List<string> Courses { get; set; } = [];
        public List<string> Instructors { get; set; } = [];
        public Dictionary<string, int> CourseIdx { get; set; } = [];
        public Dictionary<string, int> InstructorIdx { get; set; } = [];
    }
}
