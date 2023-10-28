using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

[TestFixture]
public class UserTotalStatsE2ETests
{
    private HttpClient client;

    [SetUp]
    public void Setup()
    {
        // Initialize your HttpClient and any setup here
        client = new HttpClient();
        // Ensure that your service is running at http://localhost:8080
    }

    [Test]
    public async Task GetUserTotalStats_ReturnsValidResponse()
    {
        // Arrange
        string baseUrl = "http://localhost:8080/";
        string userId = "2fba2529-c166-8574-2da2-eac544d82634";
        string apiUrl = $"{baseUrl}/api/stats/user/total?userId={userId}";

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
        // Clean up any resources, such as closing the HttpClient
        client.Dispose();
    }
}
