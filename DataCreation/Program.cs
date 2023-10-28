using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

class Program
{
    static async Task Main(string[] args)
    {
        string base_url = "https://sef.podkolzin.consulting/api/users/lastSeen";
        string outputFilePath = "Data3.json";
        string deletedUserIdsFilePath = "DeletedUserIds.json";
        int offset = 0;

        List<string> deletedUserIds = new List<string>();

        if (File.Exists(deletedUserIdsFilePath))
        {
            string deletedUserIdsData = File.ReadAllText(deletedUserIdsFilePath);
            deletedUserIds = JsonConvert.DeserializeObject<List<string>>(deletedUserIdsData);
        }

        Dictionary<string, UserData> usersData = new Dictionary<string, UserData>();
        string json1 = File.ReadAllText(outputFilePath);
        usersData = JsonConvert.DeserializeObject<Dictionary<string, UserData>>(json1);

        using var httpClient = new HttpClient();

        while (true)
        {
            UserDataResponse userDataResponse = await FetchUserDataAsync(httpClient, base_url, offset);

            if (userDataResponse == null)
            {
                Console.WriteLine("Failed to retrieve data.");
                return;
            }

            foreach (UserData user in userDataResponse.data)
            {
                // Check if the user ID is in the list of deleted user IDs
                if (deletedUserIds.Contains(user.userId))
                {
                    // Skip processing for deleted user IDs
                    continue;
                }

                if (usersData.ContainsKey(user.userId) == false)
                {
                    usersData.Add(user.userId, user);
                }

                // Process and update user data here
                ProcessUserData(usersData, user);
            }

            string jsonData = JsonConvert.SerializeObject(usersData, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(outputFilePath, jsonData);

            offset += userDataResponse.data.Count;
        }
    }


    static async Task<UserDataResponse?> FetchUserDataAsync(HttpClient httpClient, string baseUrl, int offset)
    {
        try
        {
            string apiUrl = $"{baseUrl}?offset={offset}";
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string json_data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserDataResponse>(json_data);
            }
            else
            {
                Console.WriteLine($"HTTP request failed with status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return null;
    }

    static void ProcessUserData(Dictionary<string, UserData> usersData, UserData user)
    {
        // Process and update user data here

        if (usersData.ContainsKey(user.userId))
        {
            var currUser = usersData[user.userId];
            currUser.isOnline = user.isOnline;
            currUser.lastSeenDate = user.lastSeenDate;

            if (currUser.onlineRelic == null)
            {
                currUser.onlineRelic = new List<TimeRelic>();
            }

            if (user.isOnline)
            {
                if (currUser.onlineRelic.Count != 0 && currUser.onlineRelic[^1].endDate == null)
                {
                    currUser.onlineRelic[^1].endDate = DateTime.Now.ToString();
                }
            }
            else
            {
                if (currUser.onlineRelic.Count == 0)
                {
                    currUser.onlineRelic.Add(new TimeRelic());
                    currUser.onlineRelic[^1].startDate = DateTime.Now.ToString();
                }
                else if (currUser.onlineRelic[^1].endDate != null)
                {
                    currUser.onlineRelic.Add(new TimeRelic());
                    currUser.onlineRelic[^1].startDate = DateTime.Now.ToString();
                }
            }
        }
    }
}

class UserDataResponse
{
    public List<UserData> data { get; set; }
}

class UserData
{
    public List<TimeRelic> onlineRelic { get; set; }
    public string userId { get; set; }
    public string firstName { get; set; }
    public string nickname { get; set; }
    public bool isOnline { get; set; }
    public string lastSeenDate { get; set; }
}

class TimeRelic
{
    public string startDate { get; set; }
    public string endDate { get; set; }
}
