using SigninService.Models;
using SignModel.Models;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using UDService.Services;

namespace CreateUserService.Services
{
    public class SignupService
    {
        private readonly string ApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJ1dmloZnJjYmFicGd0eHllcGZ5Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDczMTMyODAsImV4cCI6MjA2Mjg4OTI4MH0.Xnqqwg08fGbznn-Wk4kfRwU07CdVV6IecH86rpsaLl4";
        private readonly string BaseUrl = "https://buvihfrcbabpgtxyepfy.supabase.co";
        private readonly HttpClient httpc2;

        public SignupService() //constructor of the service 
        {
            httpc2 = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<bool> CreateDataAsync(string Email, string Password, string pname)
        {
            try
            {
                string endpoint = "/auth/v1/signup";

                var signUpData = new
                {
                    email = Email,
                    password = Password
                };
                string gmail = Email;
                string jsonContent = JsonSerializer.Serialize(signUpData);

                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                request.Headers.Add("apikey", ApiKey);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpc2.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Id created successfully");
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var authResponse = JsonSerializer.Deserialize<AuthResponseModel>(responseBody, options);
                    string accessToken = authResponse?.user?.email;

                    // if (!string.IsNullOrEmpty(accessToken))
                    // {
                    //     Console.WriteLine("✅ Access token retrieved: " + accessToken);
                    // }
                    // else
                    // {
                    //     Console.WriteLine($"acesss:{accessToken}");
                    // }

                    if (authResponse?.user?.id != null)
                    {
                        try
                        {
                            string puid = authResponse.user.id;

                            // INSERT into Supabase "Users" table
                            var userInsert = new
                            {
                                uid = puid,
                                name = pname,
                                email = Email,
                                favourite = (string[])null // Optional for now
                            };
                            var details = new
                            {
                                uid = puid,
                                name = pname,
                                temp = 20,
                                humidity = 75,
                                cities = (string[])null,
                                theme = false
                            };

                            string insertJson = JsonSerializer.Serialize(userInsert);

                            var insertRequest = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/profile");
                            insertRequest.Headers.Add("apikey", ApiKey);
                            insertRequest.Headers.Add("Authorization", $"Bearer {ApiKey}");
                            //insertRequest.Headers.Add("Prefer", "return=representation");
                            insertRequest.Content = new StringContent(insertJson, Encoding.UTF8, "application/json");

                            var insertResponse = await httpc2.SendAsync(insertRequest);
                            var insertResponseBody = await insertResponse.Content.ReadAsStringAsync();

                            string insertJson1 = JsonSerializer.Serialize(details);

                            var insertRequest1 = new HttpRequestMessage(HttpMethod.Post, "/rest/v1/user_details");
                            insertRequest1.Headers.Add("apikey", ApiKey);
                            insertRequest1.Headers.Add("Authorization", $"Bearer {ApiKey}");
                            //insertRequest.Headers.Add("Prefer", "return=representation");
                            insertRequest1.Content = new StringContent(insertJson1, Encoding.UTF8, "application/json");

                            var insertResponse1 = await httpc2.SendAsync(insertRequest1);
                            var insertResponseBody1 = await insertResponse.Content.ReadAsStringAsync();

                            if (insertResponse.IsSuccessStatusCode && insertResponse1.IsSuccessStatusCode)
                            {
                                Console.WriteLine("✅ User inserted into database.");
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("⚠️ Failed to insert user: " + insertResponseBody);
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
