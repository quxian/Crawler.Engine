using Crawler;
using Crawler.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler {
    public class Downloader : IDownloader<DownloadResult, List<string>> {
        private static readonly HttpClient _http = new HttpClient();
        private ConcurrentQueue<string> _urlsQueue = new ConcurrentQueue<string>();
        private bool _isStop = false;
        private Thread _doDownloadThread;
        private int _downloadThreadSleepTime;

        public Downloader(int downloadThreadSleepTime = 0) {
            _downloadThreadSleepTime = downloadThreadSleepTime;
        }

        public event Action<DownloadResult> OnDownloadResult;

        public IDownloader<DownloadResult, List<string>> AddDownloadResultEventListens(Action<DownloadResult> onDownloadResult) {
            OnDownloadResult += onDownloadResult;

            return this;
        }

        public IDownloader<DownloadResult, List<string>> AddUrls(List<string> urls) {
            urls?.ForEach(url => _urlsQueue.Enqueue(url));

            return this;
        }

        public void Dispose() {
            _isStop = true;
            _doDownloadThread.Join();

            _http.Dispose();

            SavingState();
        }

        private async void DoDownload() {
            while (!_isStop) {
                if (_urlsQueue.IsEmpty) {
                    Thread.Sleep(100);
                    continue;
                }

                var url = string.Empty;
                _urlsQueue.TryDequeue(out url);
                if (null == url || string.Empty.Equals(url))
                    continue;

                try {
                    var result = await _http.GetStringAsync(url);
                    OnDownloadResult?.Invoke(new DownloadResult { CurrentUrl = url, Result = result });
                } catch (Exception e) {

                    Console.WriteLine(new { Url = url, Exception = e });
                }

                Thread.Sleep(0);
            }
        }

        public void Run() {
            _doDownloadThread = new Thread(DoDownload);
            _doDownloadThread.Start();
        }

        public void SavingState() {
            //do
        }
    }
}
