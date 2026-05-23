using SQLite;

namespace EventHubApp.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Parish { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        // "Patron" or "Promoter"
        public string UserType { get; set; } = "Patron";
    }
}
