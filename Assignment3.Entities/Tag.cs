namespace Assignment3.Entities;

public class Tag
{
    public virtual int Id {get; set;}
    [Required]
    [StringLength(50)]
    public virtual string Name{get; set;}//Should be unique, how do we enforce that
    public virtual ICollection<Task> Tasks{get; set;}

    public Tag(String name)
    {
        Tasks = new HashSet<Task>();
        Name = name;
    }
}
