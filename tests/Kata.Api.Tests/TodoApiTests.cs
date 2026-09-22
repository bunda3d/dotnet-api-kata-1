using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Kata.Api.Tests;

public class TodoApiTests
{
	private static HttpClient CreateClient()
	{
		var factory = new WebApplicationFactory<Program>();
		return factory.CreateClient();
	}

	[Fact]
	public async Task Get_existing_todo_returns_200()
	{
		// Given
		using var client = CreateClient();

		// When
		var response = await client.GetAsync("/todos/1");

		// Then
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async Task Post_new_todo_returns_201()
	{
		// Given
		using var client = CreateClient();

		// When
		var response = await client.PostAsJsonAsync("/todos/", new { Title = "new todo" });

		// Then
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
	}

	[Fact]
	public async Task Post_new_todo_returns_correct_title()
	{
		// Given
		using var client = CreateClient();
		string? itemTitle = "New item";

		// When
		var response = await client.PostAsJsonAsync("/todos/", new { Title = itemTitle });
		response.EnsureSuccessStatusCode();

		var created = await response.Content.ReadFromJsonAsync<TodoItem>();

		// Then
		Assert.Equal(itemTitle, created?.Title);
	}


	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public async Task Post_invalid_title_returns_400(string title)
	{
		// Given
		using var client = CreateClient();

		// When
		var response = await client.PostAsJsonAsync("/todos", new { Title = title });

		// Then
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}


	[Fact]
	public async Task Delete_existing_todo_returns_204()
	{
		// Given
		using var client = CreateClient();

		// When
		var response = await client.DeleteAsync("/todos/1");

		// Then
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
	}


	[Fact]
	public async Task Delete_missing_todo_returns_404()
	{
		// Given
		using var client = CreateClient();

		// When
		var response = await client.DeleteAsync("/todos/999");

		// Then
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task Put_existing_todo_returns_204_and_persists_changes()
	{
		// Given
		using var client = CreateClient();

		// When item updated 
		var put = await client.PutAsJsonAsync("/todos/2", new { Title = "updated title", Completed = true });
		// Then item updated, returns 204
		Assert.Equal(HttpStatusCode.NoContent, put.StatusCode);
		
		// When get updated item
		var get = await client.GetAsync("/todos/2");
		var updatedItem = await get.Content.ReadFromJsonAsync<TodoItem>();
		// Then GET works, title is updated, item completion changed
		Assert.Equal(HttpStatusCode.OK, get.StatusCode);
		Assert.Equal("updated title", updatedItem?.Title);
		Assert.True(updatedItem?.Completed);
	}

	[Fact]
	public async Task Put_missing_todo_returns_404()
	{
		// Given
		using var client = CreateClient();

		// When
		var response = await client.PutAsJsonAsync("/todos/999", new { Title = "x", Completed = false });

		// Then
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}


}