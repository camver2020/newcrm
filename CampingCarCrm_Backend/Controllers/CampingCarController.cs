using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace CampingCarCrm_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampingCarController : ControllerBase
    {
        private readonly string _connectionString;

        public CampingCarController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public class CampingCar
        {
            public int CarID { get; set; }
            public string? CarNumber { get; set; }
            public string? CarModel { get; set; }
            public string? CarNickname { get; set; }
            public string? CarStatus { get; set; }
        }

        [HttpGet]
        public ActionResult<IEnumerable<CampingCar>> GetCampingCars()
        {
            var campingCars = new List<CampingCar>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT * FROM CampingCars;";
                    var cmd = new MySqlCommand(sql, conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            campingCars.Add(new CampingCar
                            {
                                CarID = reader.GetInt32("CarID"),
                                CarNumber = reader.GetString("CarNumber"),
                                CarModel = reader.IsDBNull(reader.GetOrdinal("CarModel")) ? null : reader.GetString("CarModel"),
                                CarNickname = reader.IsDBNull(reader.GetOrdinal("CarNickname")) ? null : reader.GetString("CarNickname"),
                                CarStatus = reader.IsDBNull(reader.GetOrdinal("CarStatus")) ? null : reader.GetString("CarStatus")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
            return Ok(campingCars);
        }
    }
}