namespace Assignment3.Entities;

public class Tag
{
    public virtual int id {get; init;}

    [StringLength(50)]
    public virtual string name{get; init;} //Should be unique, how do we enforce that

    public virtual Task[] tasks{get; set;}
}
