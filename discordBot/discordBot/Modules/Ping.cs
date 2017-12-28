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

        [Command("search")]
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
    }
}
