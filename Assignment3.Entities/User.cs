namespace Assignment3.Entities;

public class User
{
    public virtual int Id {get; set;}

    [Required]
    [StringLength(100)]
    public virtual string Name {get; set;}

    [Required]
    [StringLength(100)]
    public virtual string Email {get; set;}  //Should be unique, how do we enforce that? 

    public virtual ICollection<Task> Tasks {get; set;}

    public User(string name, string email)
    {
        Name = name;
        Email = email;
        Tasks = new HashSet<Task>();
    }
}
