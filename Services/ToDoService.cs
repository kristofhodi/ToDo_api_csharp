using Common;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class ToDoService(ToDoDbContext db) : IToDoService
{
    public async Task CreateAsync(ToDo entity)
    {
        entity.Created = DateTime.Now;
        db.ToDos.Add(entity);
        await db.SaveChangesAsync();
    }

    public async Task<ToDoDto> GetAsync(int id)
    {
        var entity = await db.ToDos.FirstOrDefaultAsync(e => e.Id == id);

        return new ToDoDto
        {
            Id = entity.Id,
            Created = entity.Created,
            Deadline = entity.Deadline,
            Description = entity.Description,
            IsReady = entity.IsReady,
            Title = entity.Title
        };
    }

    public async Task<List<ToDoDto>> ListAllAsync()
    {
        return await db.ToDos.Select(e => new ToDoDto
        {
            Id = e.Id,
            Created = e.Created,
            Deadline = e.Deadline,
            Description = e.Description,
            IsReady = e.IsReady,
            Title = e.Title
        }).ToListAsync();
    }

    public async Task DeleteAsync(int id)
    {
        //Régi módi törlés
        //var entity = await db.ToDos.SingleOrDefaultAsync(x => x.Id == id);
        //db.ToDos.Remove(entity);
        //await db.SaveChangesAsync();

        //Új módszer
        await db.ToDos.Where(e => e.Id == id).ExecuteDeleteAsync();
    }

    public async Task UpdateAsync(ToDo model)
    {
        //Régi módi update
        //var entity = await db.ToDos.SingleOrDefaultAsync(e => e.Id == model.Id);

        //if (entity is null)
        //    throw new Exception("Entity not found");

        //entity.Description = model.Description;
        //entity.Title = model.Title;
        //entity.Deadline = model.Deadline;
        //entity.IsReady = model.IsReady;

        //await db.SaveChangesAsync();

        //Új módszer
        await db.ToDos.Where(e => e.Id == model.Id).ExecuteUpdateAsync(
            setters =>
                setters.SetProperty(e => e.Description, model.Description)
                        .SetProperty(e => e.Title, model.Title)
                        .SetProperty(e => e.Deadline, model.Deadline)
                        .SetProperty(e => e.IsReady, model.IsReady));
    }
}
