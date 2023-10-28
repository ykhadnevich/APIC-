using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

[TestFixture]
public class UserDeleteStatsE2ETests
{
    private HttpClient client;

    [SetUp]
    public void Setup()
    {
        client = new HttpClient();
    }

    [Test]
    public async Task GetUserTotalStats_ReturnsValidResponse()
    {
        // Arrange
        string baseUrl = "http://localhost:8080/";
        string userId = "2fba2529-c166-8574-2da2-eac544d82634";
        string apiUrl = $"{baseUrl}/api/user/forget?forgetID={userId}";

        // Act
        HttpResponseMessage response = await client.GetAsync(apiUrl);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
        }
    }

    [TearDown]
    public void TearDown()
    {
        client.Dispose();
    }
}