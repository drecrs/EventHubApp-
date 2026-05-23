using SQLite;
using EventHubApp.Models;

namespace EventHubApp.Data
{
    public class AppDatabase
    {
        private SQLiteAsyncConnection? _database;

        private async Task InitAsync()
        {
            if (_database != null)
                return;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "eventhub.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<EventItem>();
        }

        public async Task<int> AddUserAsync(User user)
        {
            await InitAsync();
            return await _database!.InsertAsync(user);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            await InitAsync();
            return await _database!.Table<User>()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            await InitAsync();
            return await _database!.Table<User>()
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        public async Task<List<EventItem>> GetEventsAsync()
        {
            await InitAsync();
            return await _database!.Table<EventItem>()
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<int> AddEventAsync(EventItem eventItem)
        {
            await InitAsync();
            return await _database!.InsertAsync(eventItem);
        }

        public async Task<int> UpdateEventAsync(EventItem eventItem)
        {
            await InitAsync();
            return await _database!.UpdateAsync(eventItem);
        }

        public async Task<int> DeleteEventAsync(EventItem eventItem)
        {
            await InitAsync();
            return await _database!.DeleteAsync(eventItem);
        }
        public async Task<EventItem?> GetEventByIdAsync(int id)
        {
            await InitAsync();

            return await _database!.Table<EventItem>()
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task<int> UpdateUserAsync(User user)
        {
            await InitAsync();
            return await _database!.UpdateAsync(user);
        }



        public async Task SeedSampleEventsAsync()
        {
            await InitAsync();

            var existing = await _database!.Table<EventItem>().CountAsync();
            if (existing > 0)
                return;

            var sampleEvents = new List<EventItem>
            {
                new EventItem
                {
                    Name = "Jazz in the City",
                    Date = DateTime.Today.AddDays(5),
                    Venue = "Emancipation Park, Kingston",
                    TicketCost = 3500,
                    Sponsors = "ABC Drinks",
                    Description = "A relaxing evening of live jazz.",
                    Category = "Music"
                },
                new EventItem
                {
                    Name = "Food Festival",
                    Date = DateTime.Today.AddDays(10),
                    Venue = "Hope Gardens, Kingston",
                    TicketCost = 2000,
                    Sponsors = "Taste Jamaica",
                    Description = "A family-friendly food event.",
                    Category = "Food"
                }
            };

            await _database.InsertAllAsync(sampleEvents);
        }
    }
}
