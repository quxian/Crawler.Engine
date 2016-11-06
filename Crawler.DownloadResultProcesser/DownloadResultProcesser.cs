using Crawler;
using Crawler.Model;
using Extend;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler {
    public class DownloadResultProcesser : IDownloadResultProcesser<DownloadResult, DownloadResult, List<string>> {
        private ConcurrentQueue<DownloadResult> _downloadResultQueue = new ConcurrentQueue<DownloadResult>();
        private Thread _doProcesserThread;
        private bool _isProcesserThreadStop;

        public event Action<List<string>> OnFindAllUrls;
        public event Action<DownloadResult> OnResultToPipeline;

        public IDownloadResultProcesser<DownloadResult, DownloadResult, List<string>> AddDownloadResult(DownloadResult downloadResult) {
            _downloadResultQueue.Enqueue(downloadResult);

            return this;
        }

        public IDownloadResultProcesser<DownloadResult, DownloadResult, List<string>> AddFindAllUrlsEventListens(Action<List<string>> onFildAllUrls) {
            OnFindAllUrls += onFildAllUrls;

            return this;
        }

        public IDownloadResultProcesser<DownloadResult, DownloadResult, List<string>> AddResultToPipelineEventListens(Action<DownloadResult> onResultToPipeline) {
            OnResultToPipeline += onResultToPipeline;

            return this;
        }

        private void DoProcesser() {
            while (!_isProcesserThreadStop) {
                if (_downloadResultQueue.IsEmpty) {
                    Thread.Sleep(100);
                    continue;
                }

                DownloadResult downlaodResult = null;
                _downloadResultQueue.TryDequeue(out downlaodResult);
                if (null == downlaodResult) {
                    continue;
                }

                try {
                    var urls = downlaodResult.FindAllUrls();
                    OnFindAllUrls?.Invoke(urls);
                    OnResultToPipeline?.Invoke(downlaodResult);
                } catch (Exception e) {

                    Console.WriteLine(new { DownloadResult = downlaodResult, Exception = e });
                }

                Thread.Sleep(0);
            }
        }

        public void Run() {
            _doProcesserThread = new Thread(DoProcesser);
            _doProcesserThread.Start();

        }

        public void Dispose() {
            _isProcesserThreadStop = true;

            _doProcesserThread.Join();

            SavingState();
        }

        public void SavingState() {
            //do
        }
    }
}
