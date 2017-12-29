using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Google.Apis.Http;
using System.Net;
using System.IO;
using System.Web.Helpers;
using Newtonsoft.Json.Linq;

namespace discordBot.Modules
{
    public class Ping : ModuleBase<SocketCommandContext>
    {

        private String StocksApiKey = "";
        


       [Command("s")]
        public async Task SearchAsync([Remainder] String rquery)
        {
            String query = rquery.Trim().Replace(" ", "+").ToLower();
            String url = $"https://kgsearch.googleapis.com/v1/entities:search?query={query}&key=AIzaSyDmUlAGUw__nsFRPZ-B2UzBr-6856YPaWQ&limit=1&indent=True";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    String json = reader.ReadToEnd();
                    dynamic rData = JObject.Parse(json);
                    String firstElementInList = rData.itemListElement[0].ToString();
                    dynamic data = JObject.Parse(firstElementInList);


                    String name;
                    String briefDescription;
                    String imageUrl;
                    String detailDescription;
                    String detailDescriptionUrl;

                    try
                    {
                        name = data.result.name.ToString();
                    } catch (Exception ex)
                    {
                        name = null;
                    }

                    try
                    {
                        briefDescription = data.result.description.ToString();
                    } catch (Exception ex)
                    {
                        briefDescription = null;
                    }

                    try
                    {
                        imageUrl = data.result.image.contentUrl;
                    } catch (Exception ex)
                    {
                        imageUrl = null;
                    }

                    try
                    {
                        detailDescription = data.result.detailedDescription.articleBody;
                    } catch (Exception ex)
                    {
                        detailDescription = null;
                    }
           
                    try
                    {
                        detailDescriptionUrl = data.result.detailedDescription.url;
                    }
                    catch (Exception ex)
                    {
                        detailDescriptionUrl = null;
                    }

                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithTitle(name);
                    if (detailDescription != null)
                    {
                        builder.WithDescription(detailDescription);
                    } else if (briefDescription != null)
                    {
                        builder.WithDescription(briefDescription);
                    }
                    if (imageUrl != null)
                    {
                        builder.WithImageUrl(imageUrl);
                    }
                    if (detailDescriptionUrl != null)
                    {
                        builder.AddField("More information", detailDescriptionUrl);
                    }
                    builder.WithColor(Color.Blue);
                      
                    await ReplyAsync("", false, builder.Build());
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    Console.WriteLine("Yikes, " + errorText);
                }
            }


        }


        [Command("w")]
        public async Task WeatherAsync([Remainder] String rquery)
        {
            String query = rquery.Trim().Replace(" ", "%20").ToLower();
            String url = $"https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22{query}%22%20)%20and%20u%3D%22c%22&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    String json = reader.ReadToEnd();
                    dynamic rData = JObject.Parse(json);
                    Console.WriteLine(json);
                    String weather_number = rData.query.results.channel.item.condition.temp.ToString();
                    String weather_description = rData.query.results.channel.item.condition.text.ToString();

                    String city = rData.query.results.channel.location.city.ToString();
                    city = char.ToUpper(city[0]) + city.Substring(1); 
                    String country = rData.query.results.channel.location.country.ToString();
                    country = char.ToUpper(country[0]) + country.Substring(1);

                    String windspeed = rData.query.results.channel.wind.speed + "km/h, " + rData.query.results.channel.wind.direction + "°";
                    String windchill = rData.query.results.channel.wind.chill + "°C";
                    String humidity = rData.query.results.channel.atmosphere.humidity + "%";
                    String pressure = rData.query.results.channel.atmosphere.pressure + "mb";
                    String visibility = rData.query.results.channel.atmosphere.visibility + "km";
                    
                    EmbedBuilder builder = new EmbedBuilder();
                    
                    builder.WithTitle(city + ", " + country);

                    builder.AddInlineField("Temp", weather_number + "°C");
                    builder.AddInlineField("Condition", weather_description);

                    builder.AddInlineField("Wind Speed", windspeed);
                    builder.AddInlineField("Wind Chill", windchill);

                    builder.AddInlineField("Humidity", humidity);
                    builder.AddInlineField("Pressure", pressure);
                    builder.AddInlineField("Visibility", visibility);

                    builder.WithCurrentTimestamp();            
                    builder.WithColor(Color.DarkTeal);
                                      
                    await ReplyAsync("", false, builder.Build());
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    Console.WriteLine("Yikes, " + errorText);
                }
            }
        }
    }
}
