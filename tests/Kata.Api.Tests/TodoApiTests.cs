using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

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
}