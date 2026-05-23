using SQLite;

namespace EventHubApp.Models
{
    public class EventItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Today;
        public string Venue { get; set; } = string.Empty;
        public decimal TicketCost { get; set; }
        public string Sponsors { get; set; } = string.Empty;
        public string PromotionFilePath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public int CreatedByUserId { get; set; }
    }
}
