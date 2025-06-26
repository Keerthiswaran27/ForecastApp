namespace MFModel.Models
{
    public class FavWeather
    {
        public List<MainForecase> forecast {  get; set; }
        public bool IsFavourite { get; set; } = true;
    }
    public class MainForecase
    {
        public string CityName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public double Temp { get; set; }
        public double FeelsLike { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public int WindDeg { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Country { get; set; } = string.Empty;
        public string Main { get; set; } = string.Empty;  // short description like "Clear"
        public string Description { get; set; } = string.Empty; // full description like "clear sky"
    }
}
