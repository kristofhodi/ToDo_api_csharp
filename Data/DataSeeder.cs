using Data;

namespace Services
{
    public class DataSeeder
    {
        private readonly ToDoDbContext _db;

        public DataSeeder(ToDoDbContext db)
        {
            _db = db;
        }

        public async Task SeedAsync()
        {
            // Ha nincs adat, hozzunk létre 2 teszt ToDo-t
            if (!_db.ToDos.Any())
            {
                var todos = new[]
                {
                    new ToDo
                    {
                        Title = "Első teszt feladat",
                        Description = "Ez egy teszt adat",
                        Created = DateTime.Now,
                        IsReady = false,
                        Deadline = DateTime.Now.AddDays(7)
                    },
                    new ToDo
                    {
                        Title = "Második teszt feladat",
                        Description = "Ez is egy teszt adat",
                        Created = DateTime.Now,
                        IsReady = false,
                        Deadline = DateTime.Now.AddDays(14)
                    }
                };

                _db.ToDos.AddRange(todos);
                await _db.SaveChangesAsync();
            }
        }
    }
}

