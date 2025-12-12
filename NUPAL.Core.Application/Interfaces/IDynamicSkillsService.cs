using Nupal.Domain.Entities;

namespace NUPAL.Core.Application.Interfaces
{
    public interface IDynamicSkillsService
    {
        List<object> ExtractSkillsFromCourses(Student student);
    }
}
