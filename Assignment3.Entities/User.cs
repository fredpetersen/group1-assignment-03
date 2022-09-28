namespace Assignment3.Entities;

public class User
{
    public virtual int id {get; init;}

    [StringLength(100)]
    public virtual string name {get; set;}

    [StringLength(100)]
    public virtual string email {get; set;}  //Should be unique, how do we enforce that? 

    public virtual Task[] tasks {get; set;}
}
