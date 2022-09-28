using System.Collections.ObjectModel;

namespace Assignment3.Entities;

public class TaskRepository : ITaskRepository
{
    private readonly KanbanContext _context;

    public (Response Response, int TaskId) Create(TaskCreateDTO task)
    {
        Response response;
        var entity = new Task(task.Title);
        entity.Created = DateTime.UtcNow;
        entity.StateUpdated = DateTime.UtcNow;

        var entry = _context.Tasks.Add(entity);
        _context.SaveChanges();

        response = Created;        

        return (response, entry.Entity.Id);
    }

    public Response Delete(int taskId)
    {
        var entity = _context.Tasks.FirstOrDefault(task => task.Id == taskId);
        if(entity == null) return NotFound;
        Response response;
        switch(entity.State){
            case New:
                response = Deleted;
                _context.Tasks.Remove(entity);
                break;
            case Active:
                entity.State = Removed;
                entity.StateUpdated = DateTime.UtcNow;
                response = Deleted;
                break;
            default:
                response = Conflict;
            break;
        }
        _context.SaveChanges();
        return response;
    }

    public TaskDetailsDTO Read(int taskId)
    {
        var entity = _context.Tasks.FirstOrDefault(task => task.Id == taskId);
        if(entity == null) return null;
        var tags = new ReadOnlyCollection<string>(entity.Tags.Select(t => t.Name).ToList());
        return new TaskDetailsDTO(taskId, entity.Title, entity.Description, entity.Created, entity.AssignedTo?.Name, tags, entity.State, entity.StateUpdated);
    }

    public IReadOnlyCollection<TaskDTO> ReadAll() => new ReadOnlyCollection<TaskDTO>(_context.Tasks.Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo.Name, new ReadOnlyCollection<string>(t.Tags.Select(ta => ta.Name).ToList()),t.State)).ToList());

    public IReadOnlyCollection<TaskDTO> ReadAllByState(State state) => new ReadOnlyCollection<TaskDTO>(_context.Tasks.Where(t => t.State == state).Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo.Name, new ReadOnlyCollection<string>(t.Tags.Select(ta => ta.Name).ToList()),t.State)).ToList());

    public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag) => new ReadOnlyCollection<TaskDTO>(_context.Tasks.Where(t => t.Tags.Select(tag => tag.Name).Contains(tag)).Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo.Name, new ReadOnlyCollection<string>(t.Tags.Select(ta => ta.Name).ToList()),t.State)).ToList());

    public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId) => new ReadOnlyCollection<TaskDTO>(_context.Tasks.Where(t => t.AssignedTo.Id == userId).Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo.Name, new ReadOnlyCollection<string>(t.Tags.Select(ta => ta.Name).ToList()),t.State)).ToList());

    public IReadOnlyCollection<TaskDTO> ReadAllRemoved() => new ReadOnlyCollection<TaskDTO>(_context.Tasks.Where(t => t.State == Removed).Select(t => new TaskDTO(t.Id, t.Title, t.AssignedTo.Name, new ReadOnlyCollection<string>(t.Tags.Select(ta => ta.Name).ToList()),t.State)).ToList());

    public Response Update(TaskUpdateDTO task)
    {
        var entity = _context.Tasks.FirstOrDefault(t => t.Id == task.Id);
        if(entity == null) return NotFound;
        var assignedToUser = _context.Users.FirstOrDefault(user => user.Id == task.AssignedToId);
        if(task.AssignedToId != null && assignedToUser == null) return BadRequest;

        entity.AssignedTo = assignedToUser;
        entity.Description = task.Description;
        entity.Title = task.Title;
        entity.Tags = _context.Tags.Where(tag => task.Tags.Contains(tag.Name)).ToList();
        if(entity.State != task.State) {
            entity.State = task.State;
            entity.StateUpdated = DateTime.UtcNow;
        }

        _context.SaveChanges();

        return Updated;
    }

        public TaskRepository(KanbanContext context)
    {
        _context = context;
    }
}
