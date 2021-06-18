using GFT.Products.Service.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GFT.Products.Service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ProductsController(ILogger<ProductsController> logger, IConfiguration Configuration)
        {
            _logger = logger;
            _configuration = Configuration;
            _connectionString = _configuration["app:mysql:connection:string"];
        }

        [HttpGet]
        public IEnumerable<Product> Get()
        {
            var toReturn = new List<Product>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from Product";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            toReturn.Add(new Product()
                            {
                                Name = reader["Name"].ToString(),
                                Price = Convert.ToDouble(reader["Price"]),
                            });
                        }
                    }
                }
            }
            return toReturn;
        }

        [HttpPut]
        public async Task<int> Add([FromBody] Product productToAdd)
        {
            var toReturn = new List<Product>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText =  "insert into Product(Name,Price) values (@Name,@Price)";
                    cmd.Parameters.AddWithValue("Name", productToAdd.Name);
                    cmd.Parameters.AddWithValue("Price", productToAdd.Price);
                    return await cmd.ExecuteNonQueryAsync();

                }
            }
        }
    }
}
