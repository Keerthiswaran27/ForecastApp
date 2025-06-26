namespace HFModel.Models
{
    public class HourlyForecast
    {
        public DateTime Time { get; set; }
        public double Temperature { get; set; }
        public int WeatherCode { get; set; }
    }
}
