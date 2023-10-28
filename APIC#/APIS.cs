using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net;
using System.Text;


string baseUrl = "http://localhost:8080/";
HttpListener listener = new HttpListener();
listener.Prefixes.Add(baseUrl);
listener.Start();

Console.WriteLine("Listening for incoming requests...");

while (true)
{
    HttpListenerContext context = listener.GetContext();
    HttpListenerRequest request = context.Request;
    HttpListenerResponse response = context.Response;

    if (request.HttpMethod == "GET")
    {

        string deletedUserIdsFilePath = "DeletedUserIds.json";
        string jsonFilePath = "Data3.json";
        string dateParam = request.QueryString.Get("date");
        string userId = request.QueryString.Get("userId");
        string userId1 = request.QueryString.Get("userId1");
        string forgetID = request.QueryString.Get("forgetID");

        string toleranceParam = request.QueryString.Get("tolerance");
        string dateParam2 = request.QueryString.Get("date2"); // future date

        if (!string.IsNullOrEmpty(dateParam) && string.IsNullOrEmpty(userId))
        {
            int usersOnline = GetUserCountForDate("Data.txt", dateParam);

            if (usersOnline >= 0)
            {
                string jsonData = $"{{\"usersOnline\": {usersOnline}}}";

                response.ContentType = "application/json";
                response.ContentEncoding = Encoding.UTF8;

                byte[] buffer = Encoding.UTF8.GetBytes(jsonData);
                response.ContentLength64 = buffer.Length;

                using (Stream output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }
            }

        }

        else if (!string.IsNullOrEmpty(dateParam) && !string.IsNullOrEmpty(userId))
        {
            var userStats = GetUserStatsForDateAndUserId("Data2.txt", dateParam, userId);

            if (userStats != null)
            {
                string jsonData = $"{{\"wasUserOnline\": \"{userStats.WasUserOnline}\", \"nearestOnlineTime\": \"{userStats.NearestOnlineTime}\"}}";

                response.ContentType = "application/json";
                response.ContentEncoding = Encoding.UTF8;

                byte[] buffer = Encoding.UTF8.GetBytes(jsonData);
                response.ContentLength64 = buffer.Length;

                using (Stream output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }

        else if (!string.IsNullOrEmpty(dateParam2) && string.IsNullOrEmpty(userId))
        {
            var randomUserCount = GenerateRandomUserCount("Data.txt");
            DateTime requestedDate;
            if (DateTime.TryParseExact(dateParam2, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out requestedDate) && requestedDate > DateTime.Now)
            {
                string jsonData = $"{{\"date\": \"{dateParam2}\", \"usersOnline\": {randomUserCount}}}";

                response.ContentType = "application/json";
                response.ContentEncoding = Encoding.UTF8;

                byte[] buffer = Encoding.UTF8.GetBytes(jsonData);
                response.ContentLength64 = buffer.Length;

                using (Stream output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }


        }
        else if (string.IsNullOrEmpty(dateParam) && string.IsNullOrEmpty(toleranceParam) && !string.IsNullOrEmpty(userId))
        {
            string jsonData = File.ReadAllText(jsonFilePath);
            JObject data = JObject.Parse(jsonData);

            if (data.ContainsKey(userId) && data[userId]["onlineRelic"] is JArray onlineRelic)
            {
                long totalTime = CalculateTotalTime(onlineRelic);
                var responseJson = new { totalTime };

                byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseJson));
                response.ContentType = "application/json";
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }

        }

        else if (!string.IsNullOrEmpty(dateParam2) && !string.IsNullOrEmpty(toleranceParam) && !string.IsNullOrEmpty(userId))
        {
            DateTime specifiedDate = DateTime.Parse(dateParam2);
            double tolerance = double.Parse(toleranceParam, CultureInfo.InvariantCulture);

            bool willBeOnline = CalculateOnlineChance("Data2.txt", specifiedDate, tolerance, userId, out double onlineChance);

            string jsonResponse = $"{{\"willBeOnline\": {willBeOnline.ToString().ToLower()}, \"onlineChance\": {onlineChance}}}";

            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;

            byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);
            response.ContentLength64 = buffer.Length;

            using (Stream output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }
        }

        else if (string.IsNullOrEmpty(dateParam) && string.IsNullOrEmpty(toleranceParam) && !string.IsNullOrEmpty(userId1))
        {
            string jsonData = File.ReadAllText(jsonFilePath);
            JObject data = JObject.Parse(jsonData);

            if (data.ContainsKey(userId1) && data[userId1]["onlineRelic"] is JArray onlineRelic)
            {
                long totalTime = CalculateTotalTime(onlineRelic);

                var responseJson = new { weeklyAverage = totalTime / 7, dailyAverage = totalTime / 49 };


                response.ContentType = "application/json";
                response.SendChunked = true;

                byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseJson));
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Flush();


            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }

        else if (string.IsNullOrEmpty(dateParam) && string.IsNullOrEmpty(toleranceParam) && !string.IsNullOrEmpty(forgetID))
        {
            string jsonData = File.ReadAllText(jsonFilePath);

            JObject data = JObject.Parse(jsonData);

            if (data.ContainsKey(forgetID))
            {

                List<string> deletedUserIds = new List<string>();
                if (File.Exists(deletedUserIdsFilePath))
                {
                    string deletedUserIdsData = File.ReadAllText(deletedUserIdsFilePath);
                    deletedUserIds.Add(forgetID);
                }

                File.WriteAllText(deletedUserIdsFilePath, JsonConvert.SerializeObject(deletedUserIds));

                var responseJson = new { forgetID };

                data.Remove(userId);

                File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(data));

                response.ContentType = "application/json";

                byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseJson));
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }

        else if (string.IsNullOrEmpty(dateParam) || string.IsNullOrEmpty(toleranceParam) || string.IsNullOrEmpty(userId))
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
        }


        else
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        response.Close();
    }
    static long CalculateTotalTime(JArray onlineRelic)
    {
        long totalTime = 0;

        for (int i = 0; i < onlineRelic.Count - 1; i += 2)
        {
            var startDate = DateTime.Parse(onlineRelic[i]["startDate"].ToString());
            var endDate = DateTime.Parse(onlineRelic[i + 1]["endDate"].ToString());

            if (endDate != DateTime.MinValue)
            {
                totalTime += (long)(endDate - startDate).TotalMilliseconds;
            }
        }

        return totalTime;
    }

    int GetUserCountForDate(string dataFilePath, string requestedDate)
    {
        try
        {
            using (StreamReader reader = new StreamReader(dataFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(';');
                    if (parts.Length == 2)
                    {
                        if (parts[0].Trim() == requestedDate)
                        {
                            int usersOnline;
                            if (int.TryParse(parts[1].Trim().Split(':')[1], out usersOnline))
                            {
                                return usersOnline;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        return -1;
    }

    UserData GetUserStatsForDateAndUserId(string dataFilePath, string requestedDate, string userId)
    {
        try
        {
            using (StreamReader reader = new StreamReader(dataFilePath))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    var userData = ParseUserData(line);

                    if (userData != null && userData.Time == requestedDate && userData.ID == userId)
                    {
                        return userData;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        return null;
    }

    int GenerateRandomUserCount(string dataFilePath)
    {
        int minUserCount = int.MaxValue;
        int maxUserCount = int.MinValue;

        try
        {
            using (StreamReader reader = new StreamReader(dataFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(';');
                    if (parts.Length == 2)
                    {
                        string dataDate = parts[0].Trim();
                        if (DateTime.TryParseExact(dataDate, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                        {
                            int usersOnline;
                            if (int.TryParse(parts[1].Split(':')[1].Trim(), out usersOnline))
                            {
                                minUserCount = Math.Min(minUserCount, usersOnline);
                                maxUserCount = Math.Max(maxUserCount, usersOnline);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        if (minUserCount != int.MaxValue && maxUserCount != int.MinValue)
        {
            Random random = new Random();
            return random.Next(minUserCount, maxUserCount + 1);
        }

        return -1;
    }  
}

bool CalculateOnlineChance(string dataFilePath, DateTime specifiedDate, double tolerance, string userId, out double onlineChance)
{
    int matchingRecords = 0;
    int totalRecords = 0;

    try
    {
        using (StreamReader reader = new StreamReader(dataFilePath))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                var userData = ParseUserData(line);

                if (userData != null && userData.ID == userId)
                {
                    totalRecords++;

                    DateTime dataDate = DateTime.ParseExact(userData.Time, "dd.MM.yyyy HH:mm:ss", null);
                    TimeSpan timeDifference = specifiedDate - dataDate;

                    if (userData.WasUserOnline == "online")
                    {
                        matchingRecords++;
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }

    if (totalRecords == 0)
    {
        onlineChance = 0;
        return false;
    }

    onlineChance = (double)matchingRecords / totalRecords;

    return onlineChance >= tolerance;
}

UserData ParseUserData(string line)
{
    var userData = new UserData();
    var parts = line.Split(';');

    foreach (var part in parts)
    {
        if (part.StartsWith("ID:"))
        {
            userData.ID = part.Substring(3);
        }
        else if (part.StartsWith("Time:"))
        {
            userData.Time = part.Substring(5);
        }
        else if (part.StartsWith("nearestOnlineTime:"))
        {
            userData.NearestOnlineTime = part.Substring(18);
        }
        else if (part.StartsWith("wasUserOnline:"))
        {
            userData.WasUserOnline = part.Substring(14);
        }
    }

    return userData;
}

public class UserData
{
    public string? ID { get; set; }
    public string? Time { get; set; }
    public string? NearestOnlineTime { get; set; }
    public string? WasUserOnline { get; set; }
}

