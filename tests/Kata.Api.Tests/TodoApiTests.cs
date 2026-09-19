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
		// Given
		using var client = CreateClient();

		// When
		var response = await client.GetAsync("/todos/1");

		// Then
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}
}