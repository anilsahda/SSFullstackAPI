using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SSFullstackAPI.Data.Entities;
using SSFullstackAPI.Utilities;

namespace SSFullstackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoDatabase _db;

        public UsersController(IMongoDatabase db)
        {
            _db = db;
            _users = db.GetCollection<User>("Users");
        }

        [HttpPost]
        public IActionResult Create([FromBody] User user)
        {
            user.Id = MongoHelper.GetNextSequence(_db, "userId");
            _users.InsertOne(user);
            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_users.Find(u => true).ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(_users.Find(u => u.Id == id).FirstOrDefault());
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] User updatedUser)
        {
            updatedUser.Id = id;
            _users.ReplaceOne(u => u.Id == id, updatedUser);
            return Ok("Data updated successfully!");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _users.DeleteOne(u => u.Id == id);
            return Ok("Data deleted successfully!");
        }
    }
}