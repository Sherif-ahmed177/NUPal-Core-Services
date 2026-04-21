using MongoDB.Driver;
using Nupal.Domain.Entities;
using NUPAL.Core.Application.Interfaces;

namespace Nupal.Core.Infrastructure.Repositories
{
    public class BlockRepository : IBlockRepository
    {
        private readonly IMongoCollection<SchedulingBlock> _col;

        public BlockRepository(IMongoDatabase db)
        {
            _col = db.GetCollection<SchedulingBlock>("scheduling_blocks");

            try
            {
                // Unique index on block_id so upserts are safe
                var keys = Builders<SchedulingBlock>.IndexKeys.Ascending(x => x.BlockId);
                var opts = new CreateIndexOptions { Unique = true };
                _col.Indexes.CreateOne(new CreateIndexModel<SchedulingBlock>(keys, opts));

                // Index to speed up level queries
                var levelKeys = Builders<SchedulingBlock>.IndexKeys.Ascending(x => x.Level);
                _col.Indexes.CreateOne(new CreateIndexModel<SchedulingBlock>(levelKeys));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] BlockRepository index creation: {ex.Message}");
            }
        }

        public async Task<List<SchedulingBlock>> GetAllAsync(string? level = null)
        {
            var filter = string.IsNullOrWhiteSpace(level)
                ? Builders<SchedulingBlock>.Filter.Empty
                : Builders<SchedulingBlock>.Filter.Regex(
                    x => x.Level,
                    new MongoDB.Bson.BsonRegularExpression($"^{level.Trim()}$", "i"));

            return await _col.Find(filter)
                             .SortBy(x => x.BlockId)
                             .ToListAsync();
        }

        public async Task<SchedulingBlock?> GetByBlockIdAsync(string blockId)
        {
            var filter = Builders<SchedulingBlock>.Filter.Regex(
                x => x.BlockId,
                new MongoDB.Bson.BsonRegularExpression($"^{blockId.Trim()}$", "i"));

            return await _col.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<int> UpsertManyAsync(IEnumerable<SchedulingBlock> blocks)
        {
            var blockList = blocks.ToList();
            if (blockList.Count == 0) return 0;

            var writes = blockList.Select(block =>
            {
                var filter = Builders<SchedulingBlock>.Filter.Eq(x => x.BlockId, block.BlockId);
                return new ReplaceOneModel<SchedulingBlock>(filter, block) { IsUpsert = true };
            });

            var result = await _col.BulkWriteAsync(writes);
            return (int)(result.InsertedCount + result.ModifiedCount + result.Upserts.Count);
        }
    }
}
