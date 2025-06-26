namespace HDModel.Models
{
    public class Historydetails
    {
        public string? City { get; set; }
        public float Temp { get; set; }
        public float Hum { get; set; }
        public float Windspeed { get; set; }
        public string? Condition { get; set; }
        public DateTime Created_at { get; set; }
    }
}
