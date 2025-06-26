

namespace WRModel.Models;

public class WeatherResponse
{
    public List<ForecastItem>? List { get; set; }  // ✅ Important
    public CityInfo? City { get; set; }
}

public class ForecastItem
{

    public long Dt { get; set; }

    public MainInfo Main { get; set; }
    public List<WeatherInfo> Weather { get; set; }
    public WindInfo Wind { get; set; }
}

public class MainInfo
{
    public float Temp { get; set; }
    public float Feels_Like { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
}

public class WeatherInfo
{
    public string? Main { get; set; }
    public string? Description { get; set; }
}

public class WindInfo
{
    public float Speed { get; set; }
    public int Deg { get; set; }
}

public class CityInfo
{
    public string? Name { get; set; }
    public CoordInfo? Coord { get; set; }
    public string? Country { get; set; }
}

public class CoordInfo
{
    public float Lat { get; set; }
    public float Lon { get; set; }
}
