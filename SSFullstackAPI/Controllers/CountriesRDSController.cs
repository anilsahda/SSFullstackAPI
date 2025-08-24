using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSFullstackAPI.Data;
using SSFullstackAPI.Data.Entities;
using SSFullstackAPI.DTOs;

namespace SSFullstackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesRDSController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public CountriesRDSController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            return Ok(await _dbContext.Countries.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountry(int id)
        {
            return Ok(await _dbContext.Countries.FindAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddCountry([FromBody] CountryDTO countryDto)
        {
            var country = new Country
            {
                Name = countryDto.Name,
                Image = countryDto.Image?.FileName
            };

            _dbContext.Countries.Add(country);
            await _dbContext.SaveChangesAsync();

            return Ok("Data added successfully!");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCountry([FromBody] CountryDTO countryDto)
        {
            var country = new Country
            {
                Id = countryDto.Id,
                Name = countryDto.Name,
                Image = countryDto.Image?.FileName
            };

            _dbContext.Update(country);
            await _dbContext.SaveChangesAsync();
            return Ok("Data updated successfully!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountryById(int id)
        {
            _dbContext.Countries.Remove(await _dbContext.Countries.FindAsync(id));
            await _dbContext.SaveChangesAsync();
            return Ok("Data deleted successfully!");
        }
    }
}