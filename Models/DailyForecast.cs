
namespace DFModel.Models;

public class DailyForecast
{
    
    public string? Date { get; set; }
    public double Temp { get; set; }
    public int Humidity { get; set; }
    public string Condition { get; set; } = "";
    public double WindSpeed { get; set; }
}
