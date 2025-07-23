using System.Text.Json.Serialization;

namespace WDModel.Models
{
    public class Daily
    {
        public List<string> time { get; set; }
        public List<double> relative_humidity_2m_mean { get; set; }
    }

    public class DailyUnits
    {
        public string time { get; set; }
        public string relative_humidity_2m_mean { get; set; }
    }

    public class WeatherData
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double generationtime_ms { get; set; }
        public int utc_offset_seconds { get; set; }
        public string timezone { get; set; }
        public string timezone_abbreviation { get; set; }
        public double elevation { get; set; }
        public DailyUnits daily_units { get; set; }
        public Daily daily { get; set; }
    }
    public class Location
    {
        public string? Lat { get; set; }
        public string? Lon { get; set; }
    }

}
