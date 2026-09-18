using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITodoStore, InMemoryTodoStore>();

var app = builder.Build();

// Step 1 (intentionally broken): this should return an item when found, but always returns 404.
app.MapGet("/todos/{id:int}", (int id, ITodoStore store) =>
{
	return Results.NotFound();
});

// Step 2: Implement POST /todos
// Step 3: Implement DELETE /todos/{id:int}
// Step 4: Implement PUT /todos/{id:int}

app.Run();

public partial class Program;

public record TodoItem(int Id, string Title, bool Completed);
public record TodoCreateRequest(string Title);
public record TodoUpdateRequest(string Title, bool Completed);

public interface ITodoStore
{
	TodoItem? Get(int id);

	TodoItem Add(string title);

	bool Delete(int id);

	bool Update(int id, string title, bool completed);
}

public sealed class InMemoryTodoStore : ITodoStore
{
	private readonly ConcurrentDictionary<int, TodoItem> _items = new();
	private int _nextId = 3;

	public InMemoryTodoStore()
	{
		_items[1] = new TodoItem(1, "first item", false);
		_items[2] = new TodoItem(2, "second item", true);
	}

	public TodoItem? Get(int id) => _items.TryGetValue(id, out var item) ? item : null;

	public TodoItem Add(string title)
	{
		var id = Interlocked.Increment(ref _nextId);
		var item = new TodoItem(id, title, false);
		_items[id] = item;
		return item;
	}

	public bool Delete(int id) => _items.TryRemove(id, out _);

	public bool Update(int id, string title, bool completed)
	{
		if (!_items.ContainsKey(id)) return false;
		_items[id] = new TodoItem(id, title, completed);
		return true;
	}
}