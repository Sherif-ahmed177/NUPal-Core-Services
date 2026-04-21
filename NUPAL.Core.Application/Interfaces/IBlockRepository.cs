using Nupal.Domain.Entities;

namespace NUPAL.Core.Application.Interfaces
{
    public interface IBlockRepository
    {
        Task<List<SchedulingBlock>> GetAllAsync(string? level = null);

        Task<SchedulingBlock?> GetByBlockIdAsync(string blockId);

        Task<int> UpsertManyAsync(IEnumerable<SchedulingBlock> blocks);
    }
}
