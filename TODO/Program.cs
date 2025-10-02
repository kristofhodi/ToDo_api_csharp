using Data;
using Microsoft.EntityFrameworkCore;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Configure the Database connection
var connectionString = builder.Configuration.GetConnectionString("ToDoDbContext");
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure DI container
builder.Services.AddTransient<IToDoService, ToDoService>();

// ✅ Move this before Build()
builder.Services.AddHttpClient();

// ✅ Move this before Build()
var allowSpecificOrigins = "_allowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("*")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// ✅ Build the app AFTER registering services
var app = builder.Build();

// Run DB seeder
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
    var seeder = new Services.DataSeeder(db);
    await seeder.SeedAsync();
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(allowSpecificOrigins);
app.UseHttpsRedirection();

// Endpoint mappings
app.MapGet("get/{id:int}", async (int id, IToDoService service) =>
{
    return await service.GetAsync(id);
});

app.MapGet("list", async (IToDoService service) =>
{
    return await service.ListAllAsync();
});

app.MapPost("create", async (ToDo model, IToDoService service) =>
{
    await service.CreateAsync(model);
    return Results.Created();
});

app.MapPut("update", async (ToDo model, IToDoService service) =>
{
    await service.UpdateAsync(model);
    return Results.Ok();
});

app.MapDelete("delete/{id:int}", async (int id, IToDoService service) =>
{
    await service.DeleteAsync(id);
    return Results.Ok();
});

app.MapGet("list/pending", async (IToDoService service) =>
{
    return await service.ListPendingAsync();
});

app.Run();
