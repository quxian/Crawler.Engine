using Crawler.Model;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Crawler {
    public class Engine : IDisposable {
        private readonly IDownloader<DownloadResult, List<string>> downloader;
        private readonly IDownloadResultProcesser<DownloadResult, DownloadResult, List<string>> downloadResultProcesser;
        private readonly IScheduler<string> scheduler;
        private event Action onDispose;

        public Engine(
            IDownloader<DownloadResult, List<string>> downloader,
            IDownloadResultProcesser<DownloadResult, DownloadResult, List<string>> downloadResultProcesser,
            IScheduler<string> scheduler
            ) {
            this.downloader = downloader;
            this.downloadResultProcesser = downloadResultProcesser;
            this.scheduler = scheduler;
        }

        public Engine Run() {
            EngineCore();
            return this;
        }

        public Engine AddUrls(List<string> bootstrapUrls) {
            downloader.AddUrls(bootstrapUrls);
            return this;
        }

        public Engine AddPipeline<vNextResult>(IPipeline<DownloadResult, vNextResult> pipeline) {
            downloadResultProcesser.AddResultToPipelineEventListens(pipeline.Extract);
            onDispose += pipeline.Dispose;
            return this;
        }

        private void EngineCore() {
            downloader.AddDownloadResultEventListens(result => {
                downloadResultProcesser.AddDownloadResult(result);
            });

            downloadResultProcesser.AddFindAllUrlsEventListens(urls => {
                scheduler.AddUrls(urls);
            });

            scheduler.AddUrlDequeueEventListens(urls => {
                downloader.AddUrls(urls);
            });

            downloader.Run();
            downloadResultProcesser.Run();
            scheduler.Run();
        }

        public void Dispose() {
            downloader.Dispose();
            downloadResultProcesser.Dispose();
            scheduler.Dispose();

            onDispose?.Invoke();
        }
    }
}
