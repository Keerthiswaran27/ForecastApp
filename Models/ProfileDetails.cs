namespace PDModel.Models
{
    public class ProfileDetails
    {
        public string Email { get; set; }
        public float Temp { get; set; }
        public float Humidity { get; set; }
        public string[] Cities{ get; set; }
    }
    public class AuthDetails
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
