using System;

namespace UDService.Services;

public class Userdata
{
    public string? Username { get; set; }
    public string Uid { get; set; }
    public string Access_token { get; set; }
    public bool Theme {  get; set; }

    public async Task updateDetails(string name, string uid,string access_token,bool theme)
    {
        Username = name;
        Uid = uid;
        Access_token = access_token;
        Theme = Theme;

        await Task.Delay(100); // Simulate async work
    }
}
