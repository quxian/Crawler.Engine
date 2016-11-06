using Crawler;
using Crawler.Model;
using Extend;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler {
    public class DownloadResultProcesser : IDownloadResultProcesser<DownloadResult, DownloadResult, List<string>> {
        private ConcurrentQueue<DownloadResult> _downloadResultQueue = new ConcurrentQueue<DownloadResult>();
        private ConcurrentBag<DownloadResult> _exceptionDownoadResult = new ConcurrentBag<DownloadResult>();
        private Thread _doProcesserThread;
        private bool _isProcesserThreadStop;
        private string _savingStateFileName;

        public event Action<List<string>> OnFindAllUrls;
        public event Action<DownloadResult> OnResultToPipeline;

        public DownloadResultProcesser() {
            _savingStateFileName = $"{nameof(DownloadResultProcesser)}.txt";
            if (File.Exists(_savingStateFileName)) {
                foreach (var downloadResult in File.ReadAllLines(_savingStateFileName)) {
                    _downloadResultQueue.Enqueue(JsonConvert.DeserializeObject<DownloadResult>(downloadResult));
                }
            }
        }

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
                    _exceptionDownoadResult.Add(downlaodResult);
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
            var sb = new StringBuilder();
            foreach (var downloadResult in _downloadResultQueue.Union(_exceptionDownoadResult)) {
                sb.AppendLine(JsonConvert.SerializeObject(downloadResult));
            }
            File.WriteAllText(_savingStateFileName, sb.ToString());
            sb.Clear();
        }
    }
}
