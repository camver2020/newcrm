using System;

namespace CampingCarCrm_Frontend.Models
{
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
}