using Crawler;
using Crawler.Model;
using Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler {
    public class FindAllUrlsPipeline : AbstractPipeline<DownloadResult, List<string>> {
        public override void Extract(DownloadResult previousResult) {
            var urls = previousResult.FindAllUrls();

            _onResult(urls);
        }
    }
}
