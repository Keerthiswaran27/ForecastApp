namespace RHModel.Models
{
    public class RootHourly
    {
        public HourlyData hourly { get; set; }
    }
    public class HourlyData
    {
        public List<string> time { get; set; }
        public List<double> temperature_2m { get; set; }
        public List<int> weathercode { get; set; }
    }
}
