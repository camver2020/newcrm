using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace CampingCarCrm_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly string _connectionString;

        public MemberController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public class Member
        {
            public int MemberID { get; set; }
            public string? MemberName { get; set; }
            public string? Contact { get; set; }
            public string? EmergencyContact { get; set; }
            public string? CompanyName { get; set; }
            public string? BranchName { get; set; }
            public string? MemberMemo { get; set; }
        }

        [HttpPost]
        public IActionResult CreateMember([FromBody] Member memberData)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "INSERT INTO Members (MemberName, Contact, EmergencyContact, CompanyName, BranchName, MemberMemo) VALUES (@MemberName, @Contact, @EmergencyContact, @CompanyName, @BranchName, @MemberMemo); SELECT LAST_INSERT_ID();";
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MemberName", memberData.MemberName);
                    cmd.Parameters.AddWithValue("@Contact", memberData.Contact);
                    cmd.Parameters.AddWithValue("@EmergencyContact", memberData.EmergencyContact);
                    cmd.Parameters.AddWithValue("@CompanyName", memberData.CompanyName);
                    cmd.Parameters.AddWithValue("@BranchName", memberData.BranchName);
                    cmd.Parameters.AddWithValue("@MemberMemo", memberData.MemberMemo);

                    // 새로 추가된 회원의 ID를 바로 반환해줌
                    var newId = cmd.ExecuteScalar();
                    return Ok(new { message = "성공적으로 추가되었습니다.", memberId = newId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Member>> GetMembers()
        {
            var members = new List<Member>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT * FROM Members;";
                    var cmd = new MySqlCommand(sql, conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            members.Add(new Member
                            {
                                MemberID = reader.GetInt32("MemberID"),
                                MemberName = reader.GetString("MemberName"),
                                Contact = reader.IsDBNull(reader.GetOrdinal("Contact")) ? null : reader.GetString("Contact"),
                                EmergencyContact = reader.IsDBNull(reader.GetOrdinal("EmergencyContact")) ? null : reader.GetString("EmergencyContact"),
                                CompanyName = reader.IsDBNull(reader.GetOrdinal("CompanyName")) ? null : reader.GetString("CompanyName"),
                                BranchName = reader.IsDBNull(reader.GetOrdinal("BranchName")) ? null : reader.GetString("BranchName"),
                                MemberMemo = reader.IsDBNull(reader.GetOrdinal("MemberMemo")) ? null : reader.GetString("MemberMemo"),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
            return Ok(members);
        }
    }
}