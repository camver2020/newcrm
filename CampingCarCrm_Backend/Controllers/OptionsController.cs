using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace CampingCarCrm_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionsController : ControllerBase
    {
        private readonly string _connectionString;

        public OptionsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public class Company { public int CompanyID { get; set; } public string? CompanyName { get; set; } }
        public class Branch { public int BranchID { get; set; } public string? BranchName { get; set; } }
        public class Status { public int StatusID { get; set; } public string? StatusName { get; set; } }
        public class Manager { public int ManagerID { get; set; } public string? ManagerName { get; set; } }

        [HttpGet("companies")]
        public ActionResult<IEnumerable<Company>> GetCompanies()
        {
            var list = new List<Company>();
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT * FROM Companies ORDER BY CompanyName;", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Company { CompanyID = reader.GetInt32("CompanyID"), CompanyName = reader.GetString("CompanyName") });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            return Ok(list);
        }

        [HttpPost("companies")]
        public IActionResult AddCompany([FromBody] Company company)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("INSERT INTO Companies (CompanyName) VALUES (@Name);", conn);
                    cmd.Parameters.AddWithValue("@Name", company.CompanyName);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            return Ok(new { message = "회사가 추가되었습니다." });
        }

        [HttpDelete("companies/{id}")]
        public IActionResult DeleteCompany(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM Companies WHERE CompanyID = @ID;", conn);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            return Ok(new { message = "회사가 삭제되었습니다." });
        }

        [HttpGet("branches")]
        public ActionResult<IEnumerable<Branch>> GetBranches()
        {
            var list = new List<Branch>();
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT * FROM Branches ORDER BY BranchName;", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) { list.Add(new Branch { BranchID = reader.GetInt32("BranchID"), BranchName = reader.GetString("BranchName") }); }
                    }
                }
            }
            catch (Exception ex) { return StatusCode(500, $"An error occurred: {ex.Message}"); }
            return Ok(list);
        }

        [HttpPost("branches")]
        public IActionResult AddBranch([FromBody] Branch branch)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("INSERT INTO Branches (BranchName) VALUES (@Name);", conn);
                    cmd.Parameters.AddWithValue("@Name", branch.BranchName);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            return Ok(new { message = "지점이 추가되었습니다." });
        }

        [HttpDelete("branches/{id}")]
        public IActionResult DeleteBranch(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM Branches WHERE BranchID = @ID;", conn);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            return Ok(new { message = "지점이 삭제되었습니다." });
        }

        [HttpGet("managers")]
        public ActionResult<IEnumerable<Manager>> GetManagers()
        {
            var list = new List<Manager>();
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT * FROM Managers ORDER BY ManagerName;", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) { list.Add(new Manager { ManagerID = reader.GetInt32("ManagerID"), ManagerName = reader.GetString("ManagerName") }); }
                    }
                }
            }
            catch (Exception ex) { return StatusCode(500, $"An error occurred: {ex.Message}"); }
            return Ok(list);
        }

        [HttpPost("managers")]
        public IActionResult AddManager([FromBody] Manager manager)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("INSERT INTO Managers (ManagerName) VALUES (@Name);", conn);
                    cmd.Parameters.AddWithValue("@Name", manager.ManagerName);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            return Ok(new { message = "담당자가 추가되었습니다." });
        }

        [HttpDelete("managers/{id}")]
        public IActionResult DeleteManager(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM Managers WHERE ManagerID = @ID;", conn);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            return Ok(new { message = "담당자가 삭제되었습니다." });
        }

        [HttpGet("statuses")]
        public ActionResult<IEnumerable<Status>> GetStatuses()
        {
            var list = new List<Status>();
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT * FROM Statuses;", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) { list.Add(new Status { StatusID = reader.GetInt32("StatusID"), StatusName = reader.GetString("StatusName") }); }
                    }
                }
            }
            catch (Exception ex) { return StatusCode(500, $"An error occurred: {ex.Message}"); }
            return Ok(list);
        }
    }
}