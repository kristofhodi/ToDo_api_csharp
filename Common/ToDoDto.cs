using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common;

public record ToDoDto ()
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime Created { get; set; }
    public bool IsReady { get; set; }
}
