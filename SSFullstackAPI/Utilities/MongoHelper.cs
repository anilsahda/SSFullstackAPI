namespace SSFullstackAPI.Utilities
{
    using MongoDB.Driver;
    using SSFullstackAPI.Models;

    public static class MongoHelper
    {
        public static int GetNextSequence(IMongoDatabase db, string name)
        {
            var counters = db.GetCollection<Counter>("Counters");

            var filter = Builders<Counter>.Filter.Eq(c => c.Id, name);
            var update = Builders<Counter>.Update.Inc(c => c.Seq, 1);
            var options = new FindOneAndUpdateOptions<Counter>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var counter = counters.FindOneAndUpdate(filter, update, options);
            return counter.Seq;
        }
    }

}
