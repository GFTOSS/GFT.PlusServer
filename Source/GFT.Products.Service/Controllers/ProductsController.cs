using GFT.Products.Service.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GFT.Products.Service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string,string> _connectionStrings;

        public ProductsController(ILogger<ProductsController> logger, IConfiguration Configuration)
        {
            _logger = logger;
            _configuration = Configuration;
            _connectionStrings = new Dictionary<string, string>();
            _connectionStrings.Add("local", _configuration["app:mysql:connection:string"]);
            _connectionStrings.Add("plusserver", _configuration["app:mysql:connection:string:plusserver"]);
        }

        [HttpGet]
        [Route("{connectionName}")]
        public ActionResult<IEnumerable<Product>> Get([FromRoute] string connectionName)
        {
            var toReturn = new List<Product>();
            if(!_connectionStrings.ContainsKey(connectionName))
            {
                return NotFound($"{connectionName} not found!");
            }
            Stopwatch sw = new Stopwatch();                
            try
            {
                sw.Start();
                using (MySqlConnection conn = new MySqlConnection(_connectionStrings[connectionName]))
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
            }
            finally
            {
                sw.Stop();
                _logger.LogInformation($"{connectionName}: Loaded in {sw.Elapsed.TotalSeconds} secs.");
            }
            return Ok(toReturn);
        }

        [HttpPut]
        [Route("{connectionName}")]
        public async Task<ActionResult<int>> Add([FromRoute] string connectionName, [FromBody] Product productToAdd)
        {
            int rowsAdded = 0;
            var toReturn = new List<Product>();
            if (!_connectionStrings.ContainsKey(connectionName))
            {
                return NotFound($"{connectionName} not found!");
            }
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Start();
                using (MySqlConnection conn = new MySqlConnection(_connectionStrings[connectionName]))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "insert into Product(Name,Price) values (@Name,@Price)";
                        cmd.Parameters.AddWithValue("Name", productToAdd.Name);
                        cmd.Parameters.AddWithValue("Price", productToAdd.Price);
                        rowsAdded += await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            finally
            {
                sw.Stop();
                _logger.LogInformation($"{connectionName}: Added in {sw.Elapsed.TotalSeconds} secs.");
            }
            return Ok(rowsAdded);
        }
    }
}
