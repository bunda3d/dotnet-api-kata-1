using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITodoStore, InMemoryTodoStore>();

var app = builder.Build();

// Step 1 (intentionally broken): this should return an item when found, but always returns 404.
app.MapGet("/todos/{id:int}", (int id, ITodoStore store) =>
{
	TodoItem? item = store.Get(id);
	return item is not null ? Results.Ok(item) : Results.NotFound();
});

// Step 2: Implement POST /todos
app.MapPost("/todos", (TodoCreateRequest request, ITodoStore store) =>
{
	if (string.IsNullOrWhiteSpace(request.Title)) 
	{
		return Results.BadRequest(new { Error = "Title cannot be empty." });
	}
	var item = store.Add(request.Title);
	return Results.Created($"/todos/{item.Id}", item);
});

// Step 3: Implement DELETE /todos/{id:int}
app.MapDelete("/todos/{id:int}", (int id, ITodoStore store) =>
{
	return store.Delete(id) ? Results.NoContent() : Results.NotFound();
});

// Step 4: Implement PUT /todos/{id:int}
app.MapPut("/todos/{id:int}", (int id, TodoUpdateRequest request, ITodoStore store) =>
{
	if (string.IsNullOrWhiteSpace(request.Title))
	{
		return Results.ValidationProblem(new Dictionary<string, string[]>
		{
			[nameof(request.Title)] = ["Title is required."]
		});
	}

	return store.Update(id, request.Title.Trim(), request.Completed) 
		? Results.NoContent() 
		: Results.NotFound();
});


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
	private int _nextId = 2;

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