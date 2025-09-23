using Data;
using Microsoft.EntityFrameworkCore;

public class ToDoDbContext : DbContext
{
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {
    }

    public DbSet<ToDo> ToDos { get; set; }
}