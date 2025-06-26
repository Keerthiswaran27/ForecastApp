using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UDService.Services;

namespace USService.Services
{
    public class UserService
    {
        private readonly HttpClient httpc;
        private readonly string ApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
        private readonly string BaseUrl = "https://buvihfrcbabpgtxyepfy.supabase.co";
        private readonly Userdata _userData;

        public UserService(HttpClient httpClient, Userdata userData)
        {
            httpc = httpClient;
            _userData = userData;
        }

        public async Task<string?> UploadProfilePictureAsync(Stream fileStream, string fileName)
        {
            var fullFileName = $"{_userData.Uid}/{fileName}";
            var url = $"{BaseUrl}/storage/v1/object/profile-pics/{fullFileName}";

            using var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png"); // <-- ✅ REQUIRED

            using var content = new MultipartFormDataContent();
            content.Add(streamContent, "file", fileName);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userData.Access_token); // <-- ✅ correct token
            request.Headers.Add("apikey", ApiKey);
            request.Content = content;

            var response = await httpc.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var publicUrl = $"{BaseUrl}/storage/v1/object/public/profile-pics/{fullFileName}";
                await UpdateProfilePicPathAsync(publicUrl);
                return publicUrl;
            }
            else
            {
                Console.WriteLine("Upload failed: " + await response.Content.ReadAsStringAsync());
                return null;
            }
        }

        private async Task UpdateProfilePicPathAsync(string imageUrl)
        {
            var body = JsonSerializer.Serialize(new { profile_pic_url = imageUrl });
            var request = new HttpRequestMessage(HttpMethod.Patch, $"/rest/v1/user_details?uid=eq.{_userData.Uid}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _userData.Access_token);
            request.Headers.Add("apikey", ApiKey);
            request.Headers.Add("Prefer", "return=minimal");
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await httpc.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("DB update failed: " + await response.Content.ReadAsStringAsync());
            }
        }
    }
}
