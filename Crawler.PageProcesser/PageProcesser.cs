using Crawler.Model;
using Crawlwe;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler {
    public class PageProcesser : IPageProcesser<DownloadResult, DownloadResult, List<string>> {
        private ConcurrentQueue<DownloadResult> _downloadResultQueue = new ConcurrentQueue<DownloadResult>();



        public event Action<List<string>> OnFindAllUrls;
        public event Action<DownloadResult> OnResultToPipeline;

        public IPageProcesser<DownloadResult, DownloadResult, List<string>> AddDownloadResult(DownloadResult downloadResult) {
            _downloadResultQueue.Enqueue(downloadResult);

            return this;
        }

        public IPageProcesser<DownloadResult, DownloadResult, List<string>> AddFindAllUrlsEventListens(Action<List<List<string>>> onFildAllUrls) {
            OnFindAllUrls += OnFindAllUrls;

            return this;
        }

        public IPageProcesser<DownloadResult, DownloadResult, List<string>> AddResultToPipelineEventListens(Action<DownloadResult> onResultToPipeline) {
            OnResultToPipeline += onResultToPipeline;

            return this;
        }

        public void Dispose() {
        }

        public void Run() {
            throw new NotImplementedException();
        }
    }
}
