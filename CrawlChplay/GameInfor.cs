using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlChplay
{
    public class GameInfor
    {
        public string name { get; set; }
        public string linkImage { get; set; }
        public string developer { get; set; }
        public string fileSize { get; set; }
        public string description { get; set; }
        public string detailedDescription { get; set; }
        public string category { get; set; }
        public string linkDownload { get; set; }

        public GameInfor() { }
        public GameInfor(string name, string linkImage, string developer, string fileSize, string description, string detailedDescription, string category, string linkDownload)
        {
            this.name = name;
            this.linkImage = linkImage;
            this.developer = developer;
            this.fileSize = fileSize;
            this.description = description;
            this.detailedDescription = detailedDescription;
            this.category = category;
            this.linkDownload = linkDownload;
        }
    }
}
