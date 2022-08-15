using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyCourseFinder.Models.Mongo
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string UserId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Lon { get; set; }
        public string Lat { get; set; }
        public int TimeStart { get; set; }
        public int TimeEnd { get; set; }
    }
}
