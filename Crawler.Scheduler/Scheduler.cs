using Crawler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler {
    public class Scheduler : IScheduler<string> {
        private ConcurrentQueue<string> _urlsQueue = new ConcurrentQueue<string>();
        private ConcurrentDictionary<string, bool> _urlsDownloaded = new ConcurrentDictionary<string, bool>();

        private Thread _doSchedulingThread;
        private bool _isSchedulingThreadStop = false;

        private string _savingStateFileName_urlsQueue;
        private string _savingStateFileName_urlsDownloaded;

        public event Func<string, bool> Filter;
        public event Action<List<string>> OnUrlDequeue;

        public Scheduler() {
            _savingStateFileName_urlsQueue = $"{nameof(Scheduler)}.{nameof(_savingStateFileName_urlsQueue)}.txt";
            _savingStateFileName_urlsDownloaded = $"{nameof(Scheduler)}.{nameof(_savingStateFileName_urlsDownloaded)}.txt";
            if (File.Exists(_savingStateFileName_urlsQueue)) {
                foreach (var url in File.ReadAllLines(_savingStateFileName_urlsQueue)) {
                    _urlsQueue.Enqueue(url);
                }
            }
            if (File.Exists(_savingStateFileName_urlsDownloaded)) {
                foreach (var url in File.ReadAllLines(_savingStateFileName_urlsDownloaded)) {
                    _urlsDownloaded.TryAdd(url, true);
                }
            }

        }

        public IScheduler<string> AddUrlDequeueEventListens(Action<List<string>> onUrlDequeue) {
            OnUrlDequeue += onUrlDequeue;

            return this;
        }

        public IScheduler<string> AddUrls(List<string> urls) {
            urls?.ForEach(url => {
                if (null == Filter) {
                    _urlsQueue.Enqueue(url);
                } else {
                    if (Filter(url))
                        _urlsQueue.Enqueue(url);
                }
            });

            return this;
        }

        public IScheduler<string> AddUrlsFilter(Func<string, bool> filter) {
            Filter += filter;

            return this;
        }

        public void Dispose() {
            _isSchedulingThreadStop = true;
            _doSchedulingThread.Join();

            SavingState();
        }

        private void DoScheduling() {
            while (!_isSchedulingThreadStop) {
                if (_urlsQueue.IsEmpty) {
                    Thread.Sleep(0);
                    continue;
                }

                var url = string.Empty;
                _urlsQueue.TryDequeue(out url);
                if (null == url || string.Empty.Equals(url)) {
                    continue;
                }
                if (_urlsDownloaded.ContainsKey(url))
                    continue;

                try {
                    OnUrlDequeue?.Invoke(new List<string> { url });
                    _urlsDownloaded.TryAdd(url, true);
                } catch (Exception e) {

                    Console.WriteLine(new { Url = url, Exception = e });
                }

                Thread.Sleep(0);
            }
        }

        public void Run() {
            _doSchedulingThread = new Thread(DoScheduling);
            _doSchedulingThread.Start();
        }

        public void SavingState() {
            File.WriteAllLines(_savingStateFileName_urlsQueue, _urlsQueue);
            File.WriteAllLines(_savingStateFileName_urlsDownloaded, _urlsDownloaded.Select(kv => kv.Key));
        }
    }
}
