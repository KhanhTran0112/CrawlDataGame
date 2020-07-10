using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrawlChplay
{
    class Program
    {
        public static List<GameOverview> gameOverviews = new List<GameOverview>();
        public static List<GameInfor> gameInfors = new List<GameInfor>();
        public static string CrawlDataFromURL(string url)
        {
            string html = "";

            HttpClient httpClient = new HttpClient();

            html = httpClient.GetStringAsync(url).Result;

            return html;
        }

        public static void Game()
        {
            bool check = true;
            int a = 1;
            while (check)
            {
                string url = "https://apkpure.com/game?page=" + a + "&ajax=1";
                string html = CrawlDataFromURL(url);
                if (html.Length <= 1)
                {
                    check = false;
                }
                else
                {
                    var htmlNewAndUpdated = Regex.Matches(html, @"<div class=""category-template-img""(.+?)</span></div>", RegexOptions.Singleline);
                    for (int i = 0; i < htmlNewAndUpdated.Count; i++)
                    {
                        string UrlAndName = Regex.Match(htmlNewAndUpdated[i].ToString(), @"<div class=""category-template-title""(.+?)</a></div>", RegexOptions.Singleline).Value;
                        string urlGame = Regex.Match(UrlAndName, @""" href=""(.+?)"">", RegexOptions.Singleline).Value.Replace("\" href=\"", "").Replace("\">", "");
                        string name = Regex.Match(UrlAndName, @"href=""(.+?)</a></div>", RegexOptions.Singleline).Value.Replace("href=\"", "");
                        name = Regex.Match(name, @""">(.+?)</a></div>", RegexOptions.Singleline).Value.Replace("\">", "").Replace("</a></div>", "");
                        gameOverviews.Add(new GameOverview(urlGame, name));
                    }
                }
                a++;
                Console.WriteLine(a);
            }
            Console.WriteLine(gameOverviews.Count);
        }

        public static void GameInfor(string url)
        {
            string html = CrawlDataFromURL("https://apkpure.com"+url);

            string name = Regex.Match(html, @"<div class=""title-like""(.+?)</h1>", RegexOptions.Singleline).Value;
            name = Regex.Match(name, @"<h1>(.+?)</h1>", RegexOptions.Singleline).Value.Replace("<h1>", "").Replace("</h1>", "");
            string nameSaveImage = name.Replace(" ", "_").Replace(":", "").Replace("(", "").Replace(")", "").Replace("/", "_").Replace("|", "_");

            string linkImage = Regex.Match(html, @"<div class=""icon""(.+?)<div class=""title-like""", RegexOptions.Singleline).Value;
            linkImage = Regex.Match(linkImage, @"src=""(.+?)""></div>", RegexOptions.Singleline).Value.Replace("src=\"", "").Replace("\"></div>", "");
            DownloadImage(linkImage, "D:\\Projects\\" + nameSaveImage + ".png");
            Console.WriteLine("Download thành công");

            string developer = Regex.Match(html, @"<p itemprop=""publisher""(.+?)<div class=""ny-down""", RegexOptions.Singleline).Value;
            developer = Regex.Match(developer, @"<p itemprop=""publisher""(.+?)</a></p>", RegexOptions.Singleline).Value;
            developer = Regex.Match(developer, @"<a title="".+?"">(.+?)</a></p>", RegexOptions.Singleline).Value;
            developer = Regex.Match(developer, @""">(.+?)</a></p>", RegexOptions.Singleline).Value.Replace("\">", "").Replace("</a></p>", "");

            string fileSize = Regex.Match(html, @"<span class=""fsize""(.+?)<span itemscope itemprop=""offers""", RegexOptions.Singleline).Value;
            fileSize = Regex.Match(fileSize, @"<span>(.+?)</span>", RegexOptions.Singleline).Value.Replace("<span>", "").Replace("</span>", "");

            string description = Regex.Match(html, @"<div class=""description""(.+?)<div class=""content""", RegexOptions.Singleline).Value;
            description = Regex.Match(html, @"<h2>(.+?)</h2>", RegexOptions.Singleline).Value.Replace("<h2>", "").Replace("</h2>", "");

            string detailedDescription = Regex.Match(html, @"<div class=""content""(.+?)<div class=""showmore_trigger""", RegexOptions.Singleline).Value;
            detailedDescription = Regex.Match(detailedDescription, @""">(.+?)</div>", RegexOptions.Singleline).Value.Replace("\">", "").Replace("</div>", "").Replace("<br>", "").Replace("&#39;", "'").Replace("<b>", "").Replace("</b>", "");

            string category = Regex.Match(html, @"<div class=""additional""(.+?)</a></p>.+?", RegexOptions.Singleline).Value;
            category = Regex.Match(category, @"<p><a title="".+?<span>(.+?)</span></a></p>", RegexOptions.Singleline).Value;
            category = Regex.Match(category, @"</span> <span>(.+?)</span></a></p>", RegexOptions.Singleline).Value.Replace("</span> <span>", "").Replace("</span></a></p>", "");

            string linkDownload = Regex.Match(html, @"<div class=""ny-down""(.+?)<span class=""fsize""", RegexOptions.Singleline).Value;
            linkDownload = Regex.Match(linkDownload, @"href=""(.+?)"">", RegexOptions.Singleline).Value.Replace("href=\"", "").Replace("\">", "");

            gameInfors.Add(new GameInfor(name, linkImage, developer, fileSize, description, detailedDescription, category, linkDownload));
            
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Đang lưu dữ liệu...");
            Game();
            for(int i = 502; i < gameOverviews.Count; i++)
            {
                GameInfor(gameOverviews[i].url);
                Console.WriteLine(gameInfors[i].name);
            }
            
            Console.ReadKey();
        }
        public static void DownloadImage(string link, string address)
        {
            using(WebClient wc = new WebClient())
            {
                wc.DownloadFile(link, address);
            }
        }
    }
}
