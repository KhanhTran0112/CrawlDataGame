using Chilkat;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
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
        static string sql = @"Data Source=DESKTOP-8GEUI6U;Initial Catalog=GameAndApp;Integrated Security=True";
        public static void SaveDataBase(List<GameInfor> gameInfors, int a)
        {
            SqlConnection sqlConnection = new SqlConnection(sql);
            sqlConnection.Open();
            string cmdText = "INSERT INTO APPINFOR_TABLE(NAME, NAME_SAVE_IMAGE, LINK_IMAGE, DEVELOPER, FILE_SIZE, DESCRIPTION, DETIALED_DESCRIPTION, CATEGORY, LINK_DOWNLOAD) VALUES ('" + @gameInfors[0].name + "', '" + gameInfors[0].nameSaveImage + "', '" + gameInfors[0].linkImage + "', '" + gameInfors[0].developer + "', '" + gameInfors[0].fileSize + "', '" + gameInfors[0].description + "', '" + gameInfors[0].detailedDescription + "', '" + gameInfors[0].category + "', '" + gameInfors[0].linkDownload + "')";
            SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
            Console.WriteLine("Đã thêm thành công "+a);
        }
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
                string url = "https://apkpure.com/app?page=" + a + "&ajax=1";
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
            //527 game
            //431 app
        }

        public static void GameInfor(string url, int a)
        {
            List<GameInfor> gameInfors = new List<GameInfor>();
            string html = CrawlDataFromURL("https://apkpure.com"+url);

            string name = Regex.Match(html, @"<div class=""title-like""(.+?)</h1>", RegexOptions.Singleline).Value;
            name = Regex.Match(name, @"<h1>(.+?)</h1>", RegexOptions.Singleline).Value.Replace("<h1>", "").Replace("</h1>", "").Replace("&#39;", "");
            string nameSaveImage = name.Replace(" ", "_").Replace(":", "").Replace("(", "").Replace(")", "").Replace("/", "_").Replace("|", "_").Replace("'", "\'").Replace("_-_", "_").Replace("&#39;", "");

            string linkImage = Regex.Match(html, @"<div class=""icon""(.+?)<div class=""title-like""", RegexOptions.Singleline).Value;
            linkImage = Regex.Match(linkImage, @"src=""(.+?)""></div>", RegexOptions.Singleline).Value.Replace("src=\"", "").Replace("\"></div>", "").Replace("&#39;", "");
            DownloadImage(linkImage, "D:\\Projects\\" + nameSaveImage + ".png");
            Console.WriteLine("Download thành công "+a);

            string developer = Regex.Match(html, @"<p itemprop=""publisher""(.+?)<div class=""ny-down""", RegexOptions.Singleline).Value;
            developer = Regex.Match(developer, @"<p itemprop=""publisher""(.+?)</a></p>", RegexOptions.Singleline).Value;
            developer = Regex.Match(developer, @"<a title="".+?"">(.+?)</a></p>", RegexOptions.Singleline).Value;
            developer = Regex.Match(developer, @""">(.+?)</a></p>", RegexOptions.Singleline).Value.Replace("\">", "").Replace("</a></p>", "").Replace("&#39;", "");

            string fileSize = Regex.Match(html, @"<span class=""fsize""(.+?)<span itemscope itemprop=""offers""", RegexOptions.Singleline).Value;
            fileSize = Regex.Match(fileSize, @"<span>(.+?)</span>", RegexOptions.Singleline).Value.Replace("<span>", "").Replace("</span>", "").Replace("&#39;", "");

            string description = Regex.Match(html, @"<div class=""description""(.+?)<div class=""content""", RegexOptions.Singleline).Value;
            description = Regex.Match(html, @"<h2>(.+?)</h2>", RegexOptions.Singleline).Value.Replace("<h2>", "").Replace("</h2>", "").Replace("&#39;", "").Replace("&#39;", "");

            string detailedDescription = Regex.Match(html, @"<div class=""content""(.+?)<div class=""showmore_trigger""", RegexOptions.Singleline).Value;
            detailedDescription = Regex.Match(detailedDescription, @""">(.+?)</div>", RegexOptions.Singleline).Value.Replace("\">", "").Replace("&#39;", "").Replace("</div>", "").Replace("<br>", "").Replace("<b>", "").Replace("</b>", "").Replace("'", "");

            string category = Regex.Match(html, @"<div class=""additional""(.+?)</a></p>.+?", RegexOptions.Singleline).Value;
            category = Regex.Match(category, @"<p><a title="".+?<span>(.+?)</span></a></p>", RegexOptions.Singleline).Value;
            category = Regex.Match(category, @"</span> <span>(.+?)</span></a></p>", RegexOptions.Singleline).Value.Replace("</span> <span>", "").Replace("</span></a></p>", "").Replace("&#39;", "");

            string linkDownload = Regex.Match(html, @"<div class=""ny-down""(.+?)<span class=""fsize""", RegexOptions.Singleline).Value;
            linkDownload = Regex.Match(linkDownload, @"href=""(.+?)"">", RegexOptions.Singleline).Value.Replace("href=\"", "").Replace("\">", "");

            gameInfors.Add(new GameInfor(name, nameSaveImage, linkImage, developer, fileSize, description, detailedDescription, category, linkDownload));
            SaveDataBase(gameInfors, a);
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("Send Request");

            SendRequest();

            Console.ReadKey();
        }
        public static void DownloadImage(string link, string address)
        {
            using(WebClient wc = new WebClient())
            {
                wc.DownloadFile(link, address);
            }
        }

        public static void SendRequest()
        {
            Chilkat.HttpRequest req = new Chilkat.HttpRequest();

            req.HttpVerb = "POST";
            req.Path = @"/submission";
            req.ContentType = "multipart/form-data";

            req.AddHeader("Connection", "keep-alive");
            req.AddHeader("User-Agent", "Mozilla/5.0");
            req.AddHeader("Accept", @"*/*");

            req.AddParam("name", "khanhtv0112");
            req.AddParam("email", "khanhtv0112@gmail.com");
            req.AddParam("title", "VidMate");
            req.AddParam("category", "22");
            req.AddParam("platform", "7");
            req.AddParam("developer", "asd");
            req.AddParam("url", @"https://apkpure.com/vidmate-downloader-hd-live-tv/com.nemo.vidmate/download?from=details");
            req.AddParam("license", "asda");
            req.AddParam("file_size", "sdas");
            req.AddParam("detailed_description", "dasdfsdfsd");

            string pathToFileOnDisk = "D:\\Projects\\3DLUT_mobile.png";
            bool success = req.AddFileForUpload("image", pathToFileOnDisk);
            if (success != true)
            {
                Debug.WriteLine(req.LastErrorText);
                return;
            }

            Chilkat.Http http = new Chilkat.Http();
            Chilkat.HttpResponse resp = http.SynchronousRequest("ahihisoftware.com", 443, true, req);
            if (http.LastMethodSuccess != true)
            {
                Debug.WriteLine(http.LastErrorText);
                return;
            }
            Debug.WriteLine("HTTP response status: " + Convert.ToString(resp.StatusCode));

            string htmlStr = resp.BodyStr;
            Debug.WriteLine("Received:");
            Debug.WriteLine(htmlStr);
        }
    }
}
