using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nupal.Domain.Entities
{

    public class SchedulingBlock
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }


        [BsonElement("block_id")]
        public string BlockId { get; set; } = "";

        [BsonElement("semester")]
        public string? Semester { get; set; }

        [BsonElement("major")]
        public string? Major { get; set; }

        [BsonElement("level")]
        public string Level { get; set; } = "";

        [BsonElement("courses")]
        public List<SchedulingBlockCourse> Courses { get; set; } = new();
    }

    public class SchedulingBlockCourse
    {
        [BsonElement("course_name")]
        public string CourseName { get; set; } = "";

        [BsonElement("section")]
        public string? Section { get; set; }

        [BsonElement("type")]
        public string? Type { get; set; }

        [BsonElement("instructor")]
        public string? Instructor { get; set; }

        [BsonElement("day")]
        public string? Day { get; set; }

        [BsonElement("start_time")]
        public string? StartTime { get; set; }

        [BsonElement("end_time")]
        public string? EndTime { get; set; }

        [BsonElement("room")]
        public string? Room { get; set; }
    }
}
