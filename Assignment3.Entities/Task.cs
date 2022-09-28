namespace Assignment3.Entities;

public class Task
{
    public virtual int id {get; init;}
    
    [StringLength(100)]
    public virtual string title {get; set;}

    public virtual User? assignedTo {get; set;}

    public virtual string? description {get; set;}

    public virtual State state {get; set;}

    public virtual Tag[] tags {get; set;}
}
