using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

[TestClass]
public class IntegrationTest3
{
    private HttpClient httpClient;
    private const string baseUrl = "http://localhost:8080/"; // Update with your API's base URL

    [TestInitialize]
    public void TestInitialize()
    {
        httpClient = new HttpClient();
    }

    [TestMethod]
    public async Task TestGetPredictionForUser()
    {
        string dateParam = "15.12.2024%2015:18:39";
        string toleranceParam = "0.4";
        string userId = "2fba2529-c166-8574-2da2-eac544d82634";
        string apiUrl = $"/api/predictions/user?date2={dateParam}&tolerance={toleranceParam}&userId={userId}";
        string fullUrl = $"{baseUrl}{apiUrl}";

        HttpResponseMessage response = await httpClient.GetAsync(fullUrl);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        string responseContent = await response.Content.ReadAsStringAsync();

    }

    [TestCleanup]
    public void TestCleanup()
    {
        httpClient.Dispose();
    }
}
