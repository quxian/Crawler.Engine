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
        private int _threadPoolCount;
        private List<ManualResetEvent> _manualResetEvents = new List<ManualResetEvent>();
        private int _downloadThreadSleepTime;
        private string _savingStateFileName;
        private string _savingStateFileName_downloadedResult;

        private object _lock = new object();

        private StreamWriter streamWriter;

        public Downloader(int downloadThreadSleepTime = 0) {
            _downloadThreadSleepTime = downloadThreadSleepTime;
            _threadPoolCount = 200;

            _savingStateFileName = $"{nameof(Downloader)}.txt";
            _savingStateFileName_downloadedResult = $"{nameof(Downloader)}.{nameof(_savingStateFileName_downloadedResult)}.txt";
            streamWriter = new StreamWriter(new FileStream(_savingStateFileName_downloadedResult, FileMode.Append, FileAccess.Write));

            InitModule();
        }

        private void InitModule() {
            if (File.Exists(_savingStateFileName)) {
                File.ReadAllLines(_savingStateFileName)
                    .ToList()
                    .ForEach(url => {
                        _urlsQueue.Enqueue(url);
                    });
            }
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

            while (_threadPoolCount < 200) {
                Thread.Sleep(0);
            }

            _http.Dispose();

            SavingState();
        }

        private void DoDownload() {

            while (!_isStop) {
                if (_urlsQueue.IsEmpty) {
                    Thread.Sleep(100);
                    continue;
                }
                if (_threadPoolCount <= 0) {
                    Thread.Sleep(0);
                    continue;
                }
                --_threadPoolCount;
                var url = string.Empty;
                _urlsQueue.TryDequeue(out url);
                if (null == url || string.Empty.Equals(url))
                    continue;

                ThreadPool.QueueUserWorkItem(new WaitCallback(x => {
                    _DoDownload(url);
                    ++_threadPoolCount;
                }));

                Thread.Sleep(0);
            }
        }

        private async void _DoDownload(string url) {
            try {
                var result = await _http.GetStringAsync(url);
                var downloadResult = new DownloadResult { CurrentUrl = url, Result = result };
                OnDownloadResult?.Invoke(downloadResult);
                _downloadedResult.Enqueue(downloadResult);

                if (_downloadedResult.Count >= 1000) {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(x => SaveDownloadedResult(x)));
                }
            } catch (Exception e) {
                _exceptionUrls.Enqueue(url);
                Console.WriteLine(new { Url = url, Exception = e });
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
                    if (sb.Length >= 100000) {
                        streamWriter.Write(sb.ToString());
                        streamWriter.Flush();
                        sb.Clear();
                    }
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
