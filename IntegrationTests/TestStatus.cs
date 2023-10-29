using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Text;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

[TestClass]
public class IntegrationTest1
{
    private const string baseUrl = "http://localhost:8080/";

    [TestMethod]
    public void TestHttpGetRequest_Success()
    {
        // Simulate an HTTP GET request to the /api/stats/user endpoint with valid parameters
        string date = "12.10.2023%2011:03:25";
        string userId = "8b0b5db6-19d6-d777-575e-915c2a77959a";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{baseUrl}api/stats/user?date={date}&userId={userId}");
        request.Method = "GET";

        // Get the response from your HTTP server
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        // Check that the response status code is 200 OK
        NUnit.Framework.Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        // Read and validate the response content
        using (Stream dataStream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(dataStream))
        {
            string responseText = reader.ReadToEnd();
            Assert.IsTrue(responseText.Contains("\"wasUserOnline\":"));
            Assert.IsTrue(responseText.Contains("\"nearestOnlineTime\":"));
        }
    }

    [TestMethod]
    public void TestHttpGetRequest_BadRequest()
    {
        // Simulate an HTTP GET request to the /api/stats/user endpoint without parameters
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{baseUrl}api/stats/user");
        request.Method = "GET";

        try
        {
            // Get the response from your HTTP server
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Check that the response status code is 400 Bad Request
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        catch (WebException ex)
        {
            // If the request throws a WebException, check that it's a 400 Bad Request response
            Assert.AreEqual(HttpStatusCode.BadRequest, ((HttpWebResponse)ex.Response).StatusCode);
        }
    }
}