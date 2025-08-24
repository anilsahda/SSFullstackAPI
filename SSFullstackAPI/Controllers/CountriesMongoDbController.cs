using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SSFullstackAPI.Data.Entities;
using SSFullstackAPI.Utilities;

namespace SSFullstackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesMongoDbController : ControllerBase
    {
        private readonly IMongoCollection<Country> _countries;
        private readonly IMongoDatabase _db;

        public CountriesMongoDbController(IMongoDatabase db)
        {
            _db = db;
            _countries = db.GetCollection<Country>("Countries");
        }

        [HttpPost]
        public IActionResult Create([FromBody] Country country)
        {
            country.Id = MongoHelper.GetNextSequence(_db, "countryId");
            _countries.InsertOne(country);
            return Ok(country);
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_countries.Find(u => true).ToList());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(_countries.Find(u => u.Id == id).FirstOrDefault());
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Country country)
        {
            country.Id = id;
            _countries.ReplaceOne(u => u.Id == id, country);
            return Ok("Data updated successfully!");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _countries.DeleteOne(u => u.Id == id);
            return Ok("Data deleted successfully!");
        }
    }
}