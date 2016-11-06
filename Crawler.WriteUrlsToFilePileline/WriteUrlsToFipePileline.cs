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
    public class WriteUrlsToFilePileline : AbstractPipeline<List<string>, List<string>> {
        private readonly string _filePath;
        private readonly ConcurrentQueue<string> _urlsQueue = new ConcurrentQueue<string>();
        private long _urlsCount = 0;
        private object _lock = new object();
        private StreamWriter streamWriter;
        private StringBuilder sb = new StringBuilder(1024 * 1024);

        private List<ManualResetEvent> _manualResetEvents = new List<ManualResetEvent>();


        public WriteUrlsToFilePileline(string filePath) {
            _filePath = filePath;
            streamWriter = new StreamWriter(new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Write));

            OnDispose += () => {
                try {
                    _manualResetEvents.ForEach(thred => thred.WaitOne());
                    WriteUrlsToFile(new ManualResetEvent(false));
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            };
        }

        public override void Extract(List<string> previousResult) {
            previousResult.ForEach(url => _urlsQueue.Enqueue(url));
            if (_urlsQueue.Count > 10000 && _manualResetEvents.Count <= 64) {
                var manualResetEvent = new ManualResetEvent(false);
                _manualResetEvents.Add(manualResetEvent);
                ThreadPool.QueueUserWorkItem(new WaitCallback(WriteUrlsToFile), manualResetEvent);
            }

            _onResult(previousResult);
        }

        private void WriteUrlsToFile(object obj) {
            lock (_lock) {
                while (true) {
                    if (_urlsQueue.IsEmpty)
                        break;
                    var url = string.Empty;
                    _urlsQueue.TryDequeue(out url);
                    if (string.Empty.Equals(url) || null == url)
                        continue;
                    sb.AppendLine($"{++_urlsCount}:{url}");
                }
                streamWriter.Write(sb.ToString());
                streamWriter.Flush();
                var manualResetEvent = (ManualResetEvent)obj;
                manualResetEvent.Set();
                _manualResetEvents.Remove(manualResetEvent);

                sb.Clear();
            }
        }

    }
}
