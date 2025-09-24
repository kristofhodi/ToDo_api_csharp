namespace Common;

public record ToDoDto()
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime Created { get; set; }
    public bool IsReady { get; set; }

    public string Status => IsReady ? "kész" : "nincs kész";

    public string BackgroundColor => !IsReady && Deadline < DateTime.Now ? "#FFB3B3" : "#FFFFFF";  
}
