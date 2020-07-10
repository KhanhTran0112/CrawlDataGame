using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlChplay
{
    public class GameOverview
    {
        public string url { get; set; }
        public string name { get; set; }

        public GameOverview()
        {

        }
        public GameOverview(string url, string name)
        {
            this.url = url;
            this.name = name;
        }
    }
}
