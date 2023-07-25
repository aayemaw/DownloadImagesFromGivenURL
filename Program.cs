using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace DownloadImagesFromGivenURL
{
    class Program
    {
        static void Main(string[] args)
        {
            var websiteURL = new Uri("https://nus.edu.sg");
            var savingFilePath = "C:\\Temp\\";

            if (Directory.Exists(savingFilePath) == false)
            {
                Directory.CreateDirectory(savingFilePath);
            }

            DownloadAllImagesAsync(websiteURL, savingFilePath).Wait();
        }

        private static async Task DownloadAllImagesAsync(Uri url, string savingFilePath)
        {
            using (var client = new HttpClient() { BaseAddress = url })
            {
                string source = client.GetStringAsync(url).Result;
                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();

                document.LoadHtml(source);

                foreach (var link in document.DocumentNode.Descendants("img").Select(i => i.Attributes["src"]))
                {
                    if(link != null)
                    {
                        var fullUri = new Uri(url, link.Value);
                        var fileName = Path.GetFileName(fullUri.AbsolutePath) + (Path.HasExtension(fullUri.AbsolutePath) ? "" : ".jpg");

                        var response = await client.GetAsync(fullUri);
                        if (response != null && response.StatusCode == HttpStatusCode.OK)
                        {
                            byte[] bytes = await response.Content.ReadAsByteArrayAsync();

                            System.IO.File.WriteAllBytes(savingFilePath + fileName, bytes);
                        }
                    }                    
                }
            }
        }
    }
}
