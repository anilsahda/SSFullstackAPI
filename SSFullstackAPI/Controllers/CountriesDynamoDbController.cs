using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using SSFullstackAPI.Data.Entities;
using SSFullstackAPI.DTOs;

namespace SSFullstackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesDynamoDbController : ControllerBase
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly DynamoDBContext _dbContext;

        public CountriesDynamoDbController(IAmazonDynamoDB dynamoDb, IAmazonS3 s3Client, IConfiguration configuration)
        {
            _dynamoDb = dynamoDb;
            _s3Client = s3Client;
            _dbContext = new DynamoDBContext(_dynamoDb);
            _bucketName = configuration["AWS:S3BucketName"];
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            return Ok(await _dbContext.ScanAsync<Countries>(new List<ScanCondition>()).GetRemainingAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountry(string id)
        {
            return Ok(await _dbContext.LoadAsync<Countries>(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddCountry([FromForm] CountryDTO country)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(country.Image.FileName)}";
            using (var newMemoryStream = new MemoryStream())
            {
                await country.Image.CopyToAsync(newMemoryStream);
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = fileName,
                    BucketName = _bucketName,
                    ContentType = country.Image.ContentType
                };

                var transferUtility = new TransferUtility(_s3Client);
                await transferUtility.UploadAsync(uploadRequest);
            }

            var newCountry = new Countries
            {
                Id = Guid.NewGuid().ToString(),
                Name = country.Name,
                Image = fileName
            };

            await _dbContext.SaveAsync(newCountry);
            return Ok(newCountry);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCountry([FromForm] CountryDTO country)
        {
            var existingCountry = await _dbContext.LoadAsync<Countries>(country.Id);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(country.Image.FileName)}";

            using (var newMemoryStream = new MemoryStream())
            {
                await country.Image.CopyToAsync(newMemoryStream);
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = fileName,
                    BucketName = _bucketName,
                    ContentType = country.Image.ContentType
                };

                var transferUtility = new TransferUtility(_s3Client);
                await transferUtility.UploadAsync(uploadRequest);
            }

            existingCountry.Name = country.Name;
            existingCountry.Image = fileName;

            await _dbContext.SaveAsync(existingCountry);
            return Ok("Data updated successfully!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountryById(string id)
        {
            var country = await _dbContext.LoadAsync<Countries>(id);

            await _s3Client.DeleteObjectAsync(_bucketName, country.Image);
            await _dbContext.DeleteAsync<Countries>(id);

            return Ok("Data deleted successfully!");
        }
    }
}
