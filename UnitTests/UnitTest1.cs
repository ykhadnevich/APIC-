/*using Xunit;
using Assert = NUnit.Framework.Assert;

namespace APICSharp
{
    public class CalculateOnlineChanceTests
    {
        [Fact]
        public void CalculateOnlineChance_Returns_True_Above_Tolerance()
        {
            // Arrange
            string dataFilePath = "C:\\Users\\user\\source\\repos\\TDD\\TDD2\\Data2.txt";
            DateTime specifiedDate = new DateTime(2024, 12, 15);
            double tolerance = 0.4;
            string userId = "2fba2529-c166-8574-2da2-eac544d82634";

            // Act
            bool result = CalculateOnlineChance(dataFilePath, specifiedDate, tolerance, userId, out double onlineChance);

            // Assert
            Xunit.Assert.True(result);
            Xunit.Assert.True(onlineChance >= tolerance);

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
        }
    }
}


*/