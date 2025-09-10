using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CampingCarCrm_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly string _connectionString;

        public ReservationController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public class Reservation
        {
            public int ReservationID { get; set; }
            public int MemberID { get; set; }
            public string? MemberName { get; set; }
            public int CarID { get; set; }
            public string? CarNickname { get; set; }
            public DateTime? StartDateTime { get; set; }
            public string? ReservationStatus { get; set; }
            public string? ManagerName { get; set; }
            public string? SpecialRequest { get; set; }
            public string? OtherMemo { get; set; }
        }

        [HttpGet("bydate/{date}")]
        public ActionResult<IEnumerable<Reservation>> GetReservationsByDate(string date)
        {
            var reservations = new List<Reservation>();
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime targetDate))
            {
                return BadRequest("날짜 형식이 잘못되었습니다. 'yyyy-MM-dd' 형식으로 입력해주세요.");
            }
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string sql = @"
                        SELECT 
                            r.ReservationID, r.MemberID, m.MemberName, 
                            r.CarID, c.CarNickname, 
                            r.StartDateTime, r.ReservationStatus, 
                            r.ManagerName, r.SpecialRequest, r.OtherMemo
                        FROM Reservations r
                        LEFT JOIN Members m ON r.MemberID = m.MemberID
                        LEFT JOIN CampingCars c ON r.CarID = c.CarID
                        WHERE DATE(r.StartDateTime) = @TargetDate
                        ORDER BY r.StartDateTime ASC;";
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@TargetDate", targetDate.ToString("yyyy-MM-dd"));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservations.Add(new Reservation
                            {
                                ReservationID = reader.GetInt32("ReservationID"),
                                MemberID = reader.IsDBNull(reader.GetOrdinal("MemberID")) ? 0 : reader.GetInt32("MemberID"),
                                MemberName = reader.IsDBNull(reader.GetOrdinal("MemberName")) ? null : reader.GetString("MemberName"),
                                CarID = reader.IsDBNull(reader.GetOrdinal("CarID")) ? 0 : reader.GetInt32("CarID"),
                                CarNickname = reader.IsDBNull(reader.GetOrdinal("CarNickname")) ? null : reader.GetString("CarNickname"),
                                StartDateTime = reader.IsDBNull(reader.GetOrdinal("StartDateTime")) ? null : (DateTime?)reader.GetDateTime("StartDateTime"),
                                ReservationStatus = reader.IsDBNull(reader.GetOrdinal("ReservationStatus")) ? null : reader.GetString("ReservationStatus"),
                                ManagerName = reader.IsDBNull(reader.GetOrdinal("ManagerName")) ? null : reader.GetString("ManagerName"),
                                SpecialRequest = reader.IsDBNull(reader.GetOrdinal("SpecialRequest")) ? null : reader.GetString("SpecialRequest"),
                                OtherMemo = reader.IsDBNull(reader.GetOrdinal("OtherMemo")) ? null : reader.GetString("OtherMemo")
                            });
                        }
                    }
                }
            }
            catch (Exception ex) { return StatusCode(500, $"An error occurred: {ex.Message}"); }
            return Ok(reservations);
        }

        [HttpPost]
        public IActionResult CreateReservation([FromBody] Reservation reservationData)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "INSERT INTO Reservations (MemberID, CarID, StartDateTime, ReservationStatus, ManagerName, SpecialRequest, OtherMemo) VALUES (@MemberID, @CarID, @StartDateTime, @ReservationStatus, @ManagerName, @SpecialRequest, @OtherMemo);";
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MemberID", reservationData.MemberID);
                    cmd.Parameters.AddWithValue("@CarID", reservationData.CarID);
                    cmd.Parameters.AddWithValue("@StartDateTime", reservationData.StartDateTime);
                    cmd.Parameters.AddWithValue("@ReservationStatus", reservationData.ReservationStatus);
                    cmd.Parameters.AddWithValue("@ManagerName", reservationData.ManagerName);
                    cmd.Parameters.AddWithValue("@SpecialRequest", reservationData.SpecialRequest);
                    cmd.Parameters.AddWithValue("@OtherMemo", reservationData.OtherMemo);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { return StatusCode(500, $"An error occurred: {ex.Message}"); }
            return Ok(new { message = "새로운 예약을 성공적으로 추가했습니다." });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateReservation(int id, [FromBody] Reservation reservationData)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string sql = @"
                        UPDATE Reservations SET 
                            CarID = @CarID, 
                            ReservationStatus = @ReservationStatus, 
                            ManagerName = @ManagerName, 
                            SpecialRequest = @SpecialRequest, 
                            OtherMemo = @OtherMemo 
                        WHERE ReservationID = @ReservationID;";
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ReservationID", id);
                    cmd.Parameters.AddWithValue("@CarID", reservationData.CarID);
                    cmd.Parameters.AddWithValue("@ReservationStatus", reservationData.ReservationStatus);
                    cmd.Parameters.AddWithValue("@ManagerName", reservationData.ManagerName);
                    cmd.Parameters.AddWithValue("@SpecialRequest", reservationData.SpecialRequest);
                    cmd.Parameters.AddWithValue("@OtherMemo", reservationData.OtherMemo);
                    if (cmd.ExecuteNonQuery() == 0) return NotFound(new { message = "해당 ID의 예약을 찾을 수 없습니다." });
                }
            }
            catch (Exception ex) { return StatusCode(500, $"An error occurred: {ex.Message}"); }
            return Ok(new { message = "예약이 성공적으로 수정되었습니다." });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReservation(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "DELETE FROM Reservations WHERE ReservationID = @ID;";
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ID", id);
                    if (cmd.ExecuteNonQuery() == 0) return NotFound(new { message = "해당 ID의 예약을 찾을 수 없습니다." });
                }
            }
            catch (Exception ex) { return StatusCode(500, $"An error occurred: {ex.Message}"); }
            return Ok(new { message = "예약이 성공적으로 삭제되었습니다." });
        }
    }
}