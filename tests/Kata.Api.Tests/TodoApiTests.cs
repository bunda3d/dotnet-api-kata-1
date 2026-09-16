using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

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
		using var client = CreateClient();

		var response = await client.GetAsync("/todos/1");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async Task Post_creates_todo_returns_201()
	{
		using var client = CreateClient();

		var response = await client.PostAsJsonAsync("/todos", new { title = "practice kata" });

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
	}

	[Fact]
	public async Task Put_updates_todo_returns_204()
	{
		using var client = CreateClient();

		var response = await client.PutAsJsonAsync("/todos/1", new { title = "updated", completed = true });

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
	}

	[Fact]
	public async Task Delete_existing_todo_returns_204()
	{
		using var client = CreateClient();

		var response = await client.DeleteAsync("/todos/1");

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
	}
}