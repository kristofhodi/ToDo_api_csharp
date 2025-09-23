namespace Data;

public class ToDo
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime Created { get; set; }
    public bool IsReady { get; set; }
}
