using Crawler;
using Crawler.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler {
    public class Downloader : IDownloader<DownloadResult, List<string>> {
        private static readonly HttpClient _http = new HttpClient();
        private ConcurrentQueue<string> _urlsQueue = new ConcurrentQueue<string>();
        private ConcurrentQueue<string> _exceptionUrls = new ConcurrentQueue<string>();
        private ConcurrentQueue<DownloadResult> _downloadedResult = new ConcurrentQueue<DownloadResult>();
        private bool _isStop = false;
        private Thread _doDownloadThread;
        private int _downloadThreadSleepTime;
        private string _savingStateFileName;
        private string _savingStateFileName_downloadedResult;

        private object _lock = new object();

        private StreamWriter streamWriter;

        public Downloader(int downloadThreadSleepTime = 0) {
            _downloadThreadSleepTime = downloadThreadSleepTime;
            _savingStateFileName = $"{nameof(Downloader)}.txt";
            _savingStateFileName_downloadedResult = $"{nameof(Downloader)}.{nameof(_savingStateFileName_downloadedResult)}.txt";
            if (File.Exists(_savingStateFileName)) {
                File.ReadAllLines(_savingStateFileName)
                    .ToList()
                    .ForEach(url => {
                        _urlsQueue.Enqueue(url);
                    });
            }

            streamWriter = new StreamWriter(new FileStream(_savingStateFileName_downloadedResult, FileMode.Append, FileAccess.Write));
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
                    var downloadResult = new DownloadResult { CurrentUrl = url, Result = result };
                    OnDownloadResult?.Invoke(downloadResult);
                    _downloadedResult.Enqueue(downloadResult);
                    if (_downloadedResult.Count >= 1000) {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(x => { }));
                    }
                } catch (Exception e) {
                    _exceptionUrls.Enqueue(url);
                    Console.WriteLine(new { Url = url, Exception = e });
                }

                Thread.Sleep(0);
            }
        }

        public void Run() {
            _doDownloadThread = new Thread(DoDownload);
            _doDownloadThread.Start();
        }

        private void SaveDownloadedResult(object obj) {
            lock (_lock) {
                var sb = new StringBuilder();
                while (!_downloadedResult.IsEmpty) {
                    DownloadResult downloadedReuslt = null;
                    _downloadedResult.TryDequeue(out downloadedReuslt);
                    if (null != downloadedReuslt)
                        sb.AppendLine(JsonConvert.SerializeObject(downloadedReuslt));
                }
                streamWriter.Write(sb.ToString());
                streamWriter.Flush();
                sb.Clear();
            }
        }
        public void SavingState() {
            SaveDownloadedResult(null);
            File.WriteAllLines(_savingStateFileName, _urlsQueue.Union(_exceptionUrls));
        }
    }
}
