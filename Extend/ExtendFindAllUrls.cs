using Crawler.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extend {
    public static class ExtendFindAllUrls {
        public static List<string> FindAllUrls(this DownloadResult downloadResult) {
            var doc = new HtmlDocument();
            doc.LoadHtml(downloadResult.Result);

            var baseAddress = new Uri(downloadResult.CurrentUrl);

            var urls = _FindAllUrls(doc)?.Select(url => {
                if (url.IndexOf("http") == 0)
                    return url;
                if (url.Contains("javascript") || url.IndexOf("http") > 0)
                    return null;
                var combineUrl = new Uri(baseAddress, url);
                return combineUrl.ToString();
            }).Where(url => null != url);

            return urls?.ToList();
        }

        public static List<string> FindAllUrls(this HtmlDocument doc) {
            return _FindAllUrls(doc)?.ToList();
        }

        private static IEnumerable<string> _FindAllUrls(HtmlDocument doc) {
            return doc?.DocumentNode?.SelectNodes("//a[@href]")?.Select(link => link?.Attributes["href"]?.Value);
        }
    }
}
