using Assignment3.Core;
namespace Assignment3.Entities.Tests;

public sealed class TagRepositoryTests : IDisposable
{
    private readonly KanbanContext _context;
    private readonly TagRepository _repository;
    public TagRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.Tags.AddRange(new Tag("High priority") { Id = 1 }, new Tag("Low priority") { Id = 2 });
        context.SaveChanges();

        _context = context;
        _repository = new TagRepository(_context);
    }

    [Fact]
    public void Delete_given_non_existing_Id_should_return_NotFound() => _repository.Delete(100).Should().Be(NotFound);

    [Fact]
    public void Update_given_non_existing_Id_should_return_NotFound() => _repository.Update(new TagUpdateDTO(100, "Critical")).Should().Be(NotFound);

    [Fact]
    public void Find_given_non_exisiting_Id_should_return_null() => _context.Tags.Find(3).Should().Be(null);

    public void Dispose()
    {
        _context.Dispose();
    }
}
