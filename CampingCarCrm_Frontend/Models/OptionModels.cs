namespace CampingCarCrm_Frontend.Models
{
    public class Company
    {
        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }
    }

    public class Branch
    {
        public int BranchID { get; set; }
        public string? BranchName { get; set; }
    }

    public class Status
    {
        public int StatusID { get; set; }
        public string? StatusName { get; set; }
    }

    public class Manager
    {
        public int ManagerID { get; set; }
        public string? ManagerName { get; set; }
    }
}