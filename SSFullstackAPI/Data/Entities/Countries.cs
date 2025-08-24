using Amazon.DynamoDBv2.DataModel;

namespace SSFullstackAPI.Data.Entities
{
    [DynamoDBTable("Countries")]
    public class Countries
    {
        [DynamoDBHashKey]
        public int Id { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public string Image { get; set; }
    }
}
