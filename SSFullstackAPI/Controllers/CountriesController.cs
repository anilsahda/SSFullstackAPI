using Microsoft.AspNetCore.Mvc;
using SSFullstackAPI.Data;
using SSFullstackAPI.Data.Entities;
using SSFullstackAPI.DTOs;

namespace SSFullstackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CountriesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult GetCountries()
        {
            return Ok(_context.Countries.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetCountry(int id)
        {
            return Ok(_context.Countries.Find(id));
        }

        [HttpPost]
        public IActionResult AddCountry([FromForm] CountryDTO country)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(country.Image.FileName)}";
            var path = Path.Combine(_env.WebRootPath, "api/Uploads", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using var stream = new FileStream(path, FileMode.Create);
            country.Image.CopyTo(stream);

            _context.Countries.Add(new Country() { Name = country.Name, Image = fileName });
            _context.SaveChanges();

            return Ok(country);
        }

        [HttpPut]
        public IActionResult UpdateCountry([FromForm] CountryDTO country)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(country.Image.FileName)}";
            var path = Path.Combine(_env.WebRootPath, "api/Uploads", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using var stream = new FileStream(path, FileMode.Create);
            country.Image.CopyTo(stream);

            _context.Countries.Update(new Country() { Id = country.Id, Name = country.Name, Image = fileName });
            _context.SaveChanges();
            return Ok("Data updated successfully!");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCountryById(int id)
        {
            var country = _context.Countries.Find(id);
            System.IO.File.Delete(Path.Combine(_env.WebRootPath, "api/Uploads", country.Image));
            _context.Countries.Remove(country);
            _context.SaveChanges();
            return Ok("Data deleted successfully!");
        }
    }
}