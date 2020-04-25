using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Xml;
using Microsoft.Azure.Cosmos.Table;

namespace IlmoitusTaulu
{
    class Program
    {
        static ITelegramBotClient _botClient;
        private static User _botUser;
        private static CultureInfo _cinfo;
        private static IConfigurationRoot _config;

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            //Set config file
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            _config = builder.Build();

            //Init bot client
            _botClient = new TelegramBotClient(_config["accessToken"]);
            _botUser = _botClient.GetMeAsync().Result;
            _cinfo = new CultureInfo(_config["cultureInfo"]);

            //Azure storage connection
            var storageConnectionString = _config["storageConnectionString"];
            CloudStorageAccount storageAccount;

            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("rsstable");
                await table.CreateIfNotExistsAsync();
                var previous = await Utils.RetrieveEntityUsingPointQueryAsync(table);

                if(previous == null)
                {
                    previous = new Model.DateTimeEntity();
                }

                //Read RSS feed
                var reader = XmlReader.Create(_config["rssFeed"]);
                var feed = SyndicationFeed.Load(reader);

                var newPosts = feed.Items.Where(i => i.PublishDate > previous.Timestamp).OrderBy(i => i.PublishDate).ToList();

                foreach(var np in newPosts)
                {
                    var message = np.ToMessageString(_cinfo);
                    await _botClient.SendTextMessageAsync(_config["channelName"], message, ParseMode.Html);
                }

                //First one of feed is the latest one
                previous.Timestamp = feed.Items.First().PublishDate.DateTime;
                await Utils.InsertTimeStamp(table, previous);

            }
        }
    }

    public static class Extensions
    {
        public static string FormatText(this string text)
        {
            var result = System.Net.WebUtility.HtmlDecode(text);
            return Regex.Replace(result, "<.*?>", String.Empty).Trim();
        }

        public static string ToMessageString(this SyndicationItem item, CultureInfo cinfo)
        {
            //Title
            var message = "<b>" + item.Title.Text.FormatText() + "</b> \n\n";

            //Body
            message += item.Summary.Text.FormatText() + "\n\n";

            //PubDate
            message += "Julkaistu " + item.PublishDate.DateTime.ToString(cinfo) + "\n";

            //Link
            message += item.Links[0].Uri;

            return message;
        }
    }
}