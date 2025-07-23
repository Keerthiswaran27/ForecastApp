namespace DWModel.Models
{
    public class DailyWeather
    {
        public DateTime Date { get; set; }
        public double Tavg { get; set; }
        public double Tmin { get; set; }
        public double Tmax { get; set; }
        public double Prcp { get; set; }
        public double Wspd { get; set; }
        public double Pres { get; set; }
    }

}
