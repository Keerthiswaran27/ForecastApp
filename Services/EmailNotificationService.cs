﻿using MFModel.Models;
        using Microsoft.JSInterop;
using RSService.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using URService.Services;
using UDService.Services;
namespace ENService.Services
{
    public class EmailNotificationService
    {
        private readonly IJSRuntime _js;
        private readonly RegisterService _registerService;
        private readonly Userdata _userdata;
        public EmailNotificationService(IJSRuntime js, RegisterService registerService,Userdata userData)
        {
            _js = js;
            _registerService = registerService;
            _userdata = userData;

        }
        private List<FavWeather> weatherfav = new();
        public async Task SendWeatherAlertAsync(string toEmail, float temp, float hum, List<string> place)
        {
            weatherfav.Clear();
            foreach (var city in place)
            {
                var fav = new FavWeather();
                fav.forecast = await _registerService.GetForecastAsync(city, 5); // Make sure method exists
                weatherfav.Add(fav);
            }

            //foreach (var a in weatherfav)
            //{
            //    Console.WriteLine(a.forecast[0].CityName);
            //}
            

            foreach (var city in weatherfav)
            {
                
   
                if ((city.forecast[0].Temp >= 15 && city.forecast[0].Temp <= 25) || (city.forecast[0].Humidity >= 80))
                {
                    var message = $"Its gonna rain in {city.forecast[0].CityName}, Take an umbrella";
                    var templateParams = new Dictionary<string, object>
                    {
                        { "to_email", toEmail },
                        { "name", "Weather Bloom" },
                        {"user_name",_userdata.Username },
                        { "time", DateTime.Now.ToString("f") },
                        { "alert_message", message },
                        { "location", city.forecast[0].CityName },
                        { "temperature",city.forecast[0].Temp  },
                        { "humidity", city.forecast[0].Humidity },
                        { "wind_speed", city.forecast[0].WindSpeed }
                    };
                    await _js.InvokeVoidAsync("sendWeatherEmail",
                                "service_bvykk0s",
                                "template_1lpea97",
                                templateParams);
                }
            }
        }
    }
}


