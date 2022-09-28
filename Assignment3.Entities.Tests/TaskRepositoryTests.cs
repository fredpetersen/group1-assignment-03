using Assignment3.Core;

namespace Assignment3.Entities.Tests;

public class TaskRepositoryTests : IDisposable
{
    private readonly KanbanContext _context;
    private readonly TaskRepository _repository;
    public TaskRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.Tasks.AddRange(new Task("Clean dishes") { Id = 1 }, new Task("do BDSA assignment") { Id = 2 });
        context.SaveChanges();

        _context = context;
        _repository = new TaskRepository(_context);
    }

    [Fact]
    public void Delete_given_non_existing_Id_should_return_NotFound() => _repository.Delete(100).Should().Be(NotFound);

    [Fact]
    public void Update_given_non_existing_Id_should_return_NotFound() => _repository.Update(new TaskUpdateDTO(100, "Do work", 1, "Work should be done", new HashSet<string>(){"High priority"}, Active)).Should().Be(NotFound);

    [Fact]
    public void Find_given_non_exisiting_Id_should_return_null() => _context.Tags.Find(3).Should().Be(null);

    [Fact]
    public void Delete_given_new_task_should_remove_from_database()
    {
        var obj = _context.Tasks.Find(1)!;
        obj.State = New;

        var response = _repository.Delete(1);

       response.Should().Be(Deleted);
    }

    [Fact]
    public void Delete_given_active_task_should_change_state_to_removed()
    {
        var obj = _context.Tasks.Find(1)!;
        obj.State = Active;

        _repository.Delete(1);

        obj.State.Should().Be(Removed);
    }

    [Fact]
    public void Delete_given_removed_task_should_return_Conflict()
    {
        var obj = _context.Tasks.Find(1)!;
        obj.State = Removed;

        var response = _repository.Delete(1);

        response.Should().Be(Conflict);
    }

    [Fact]
    public void Create_sets_State_of_new_task_to_new()
    {
        var (response, newTaskId) = _repository.Create(new TaskCreateDTO("Test kode", null, null, new HashSet<string>()));
        var obj = _context.Tasks.Find(newTaskId)!;

        obj.State.Should().Be(New);
        obj.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        obj.StateUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Update_when_updating_state_sets_new_StateUpdated_datetime()
    {
        var obj = _context.Tasks.Find(1)!;
        obj.StateUpdated = DateTime.UtcNow.AddSeconds(-10);
        
        _repository.Update(new TaskUpdateDTO(1, "Clean dishes", null, null, new HashSet<string>(), Removed));

        obj.StateUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
