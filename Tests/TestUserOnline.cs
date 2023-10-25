using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

[TestClass]
public class IntegrationTests
{
    private HttpClient httpClient;
    private const string baseUrl = "http://localhost:8080";

    [TestInitialize]
    public void TestInitialize()
    {
        httpClient = new HttpClient();
    }

    [TestMethod]
    public async Task TestGetUsersStatsWithValidDate()
    {

        string dateParam = "15.10.2024%2015:18:39"; // URL-encoded date
        string apiUrl = $"/api/stats/users?date2={dateParam}";
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
