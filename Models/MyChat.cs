using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;

namespace FileUpload.Models
{
    public class MyChat
    {
        [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public Guid Id { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Message")]
        public string Message { get; set; }
        [BsonElement("Date")]
        public DateTime Date { get; set; }
    }
}
